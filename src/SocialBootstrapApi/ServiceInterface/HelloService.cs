using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ServiceStack;

namespace SocialBootstrapApi.ServiceInterface
{
    //Define Request and Response DTOs
    [Route("/hellotext/{Name}")]
    public class HelloText
    {
        public string Name { get; set; }
    }

    [Route("/hellohtml/{Name}")]
    public class HelloHtml
    {
        public string Name { get; set; }
    }

    [Route("/helloimage/{Name}")]
    public class HelloImage
    {
        public string Name { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? FontSize { get; set; }
        public string Foreground { get; set; }
        public string Background { get; set; }
    }

    [Route("/hello/{Name}")]
    public class Hello : IReturn<HelloResponse>
    {
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }

    //Implementation
    public class HelloService : Service
    {
        public object Get(HelloHtml request)
        {
            return "<h1>Hello, {0}!</h1>".Fmt(request.Name);
        }

        [AddHeader(ContentType = "text/plain")]
        public object Get(HelloText request)
        {
            return "<h1>Hello, {0}!</h1>".Fmt(request.Name);
        }

        [AddHeader(ContentType = "image/png")]
        public object Get(HelloImage request)
        {
            var width = request.Width.GetValueOrDefault(640);
            var height = request.Height.GetValueOrDefault(360);
            var bgColor = request.Background != null ? Color.FromName(request.Background) : Color.ForestGreen;
            var fgColor = request.Foreground != null ? Color.FromName(request.Foreground) : Color.White;

            var image = new Bitmap(width, height);
            using (var g = Graphics.FromImage(image))
            {
                g.Clear(bgColor);

                var drawString = "Hello, {0}!".Fmt(request.Name);
                var drawFont = new Font("Times", request.FontSize.GetValueOrDefault(40));
                var drawBrush = new SolidBrush(fgColor);
                var drawRect = new RectangleF(0, 0, width, height);

                var drawFormat = new StringFormat {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };

                g.DrawString(drawString, drawFont, drawBrush, drawRect, drawFormat);

                var ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                return ms;
            }
        }

        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Hello, {0}!".Fmt(request.Name) };

            //C# client can call with:
            //var response = client.Get(new Hello { Name = "ServiceStack" });
        }
    }
}