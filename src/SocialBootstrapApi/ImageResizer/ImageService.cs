using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Funq;
using ServiceStack.Common;
using ServiceStack.Common.Utils;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using System.Drawing;
using System.Drawing.Drawing2D;

//Entire C# source code for ImageResizer backend - there is no other .cs :)
namespace SocialBootstrapApi.ImageResizer
{
    [Route("/upload")]
    public class Upload
    {
        public string Url { get; set; }
    }

    [Route("/images")]
    public class Images { }

    [Route("/resize/{Id}")]
    public class Resize
    {
        public string Id { get; set; }
        public string Size { get; set; }
    }

    [Route("/reset")]
    public class Reset { }

    public class ImageService : Service
    {
        const int ThumbnailSize = 100;
        readonly string UploadsDir = "~/uploads".MapHostAbsolutePath();
        readonly string ThumbnailsDir = "~/uploads/thumbnails".MapHostAbsolutePath();

        public object Get(Images request)
        {
            return Directory.GetFiles(UploadsDir).SafeConvertAll(x => x.SplitOnLast(Path.DirectorySeparatorChar).Last());
        }

        public object Post(Upload request)
        {
            if (request.Url != null)
            {
                using (var ms = new MemoryStream(request.Url.GetBytesFromUrl()))
                {
                    WriteImage(ms);
                }
            }

            foreach (var uploadedFile in RequestContext.Files.Where(uploadedFile => uploadedFile.ContentLength > 0))
            {
                using (var ms = new MemoryStream())
                {
                    uploadedFile.WriteTo(ms);
                    WriteImage(ms);
                }
            }

            return HttpResult.Redirect("/ImageResizer/");
        }

        private void WriteImage(Stream ms)
        {
            var hash = GetMd5Hash(ms);

            ms.Position = 0;
            var fileName = hash + ".png";
            using (var img = Image.FromStream(ms))
            {
                img.Save(UploadsDir.CombineWith(fileName));

                var stream = Resize(img, ThumbnailSize, ThumbnailSize);
                File.WriteAllBytes(ThumbnailsDir.CombineWith(fileName), stream.ReadFully());
            }
        }

        [AddHeader(ContentType = "image/jpg")]
        public object Get(Resize request)
        {
            var imagePath = UploadsDir.CombineWith(request.Id + ".png");
            if (request.Id == null || !File.Exists(imagePath))
                throw HttpError.NotFound(request.Id + " was not found");

            using (var stream = File.OpenRead(imagePath))
            using (var img = Image.FromStream(stream))
            {

                var parts = request.Size == null ? null : request.Size.Split('x');
                int width = img.Width;
                int height = img.Height;

                if (parts != null && parts.Length > 0)
                    int.TryParse(parts[0], out width);

                if (parts != null && parts.Length > 1)
                    int.TryParse(parts[1], out height);

                return Resize(img, width, height);
            }
        }

        public static string GetMd5Hash(Stream stream)
        {
            var hash = MD5.Create().ComputeHash(stream);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static Stream Resize(Image img, int newWidth, int newHeight)
        {
            if (newWidth != img.Width || newHeight != img.Height)
            {
                var ratioX = (double)newWidth / img.Width;
                var ratioY = (double)newHeight / img.Height;
                var ratio = Math.Max(ratioX, ratioY);

                var width = (int)(img.Width * ratio);
                var height = (int)(img.Height * ratio);

                var newImage = new Bitmap(width, height);
                Graphics.FromImage(newImage).DrawImage(img, 0, 0, width, height);
                img = newImage;

                if (img.Width != newWidth || img.Height != newHeight)
                {
                    var startX = (Math.Max(img.Width, newWidth) - Math.Min(img.Width, newWidth)) / 2;
                    var startY = (Math.Max(img.Height, newHeight) - Math.Min(img.Height, newHeight)) / 2;
                    img = Crop(img, newWidth, newHeight, startX, startY);
                }
            }

            var ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return ms;
        }

        public static Image Crop(Image Image, int newWidth, int newHeight, int startX = 0, int startY = 0)
        {
            if (Image.Height < newHeight)
                newHeight = Image.Height;

            if (Image.Width < newWidth)
                newWidth = Image.Width;

            using (var bmp = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb))
            {
                bmp.SetResolution(72, 72);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(Image, new Rectangle(0, 0, newWidth, newHeight), startX, startY, newWidth, newHeight, GraphicsUnit.Pixel);

                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        Image.Dispose();
                        var outimage = Image.FromStream(ms);
                        return outimage;
                    }
                }
            }
        }

        public object Any(Reset request)
        {
            Directory.GetFiles(UploadsDir).ToList().ForEach(File.Delete);
            Directory.GetFiles(ThumbnailsDir).ToList().ForEach(File.Delete);
            return HttpResult.Redirect("/ImageResizer/");
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("Image Resizer", typeof(AppHost).Assembly) { }
        public override void Configure(Container container) { }
    }

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }
    }
}