#define UseEmbeddedHtmlForm

using ServiceStack;

namespace SocialBootstrapApi.ServiceInterface
{
    [Route("/login")]
    public class Login
    {
        public string Redirect { get; set; }
    }

    public class LoginService : Service
    {
#if UseEmbeddedHtmlForm
        const string HtmlBody = @"<!DOCTYPE html>
<html lang=""en"">
<head><title>Login Page</title></head>
<style>body {{ font: 16px/24px Arial; }}</style>
<body>
    <h3>Login Page</h3>
    <p>
        Using the Embedded HTML Form in the <a href='https://github.com/ServiceStack/SocialBootstrapApi/blob/master/src/SocialBootstrapApi/ServiceInterface/LoginService.cs'>/login</a> service.
    </p>
    <p>
        Autentication is required to view: <b>{0}</b>
    </p>
    <form action='/api/auth/credentials' method='POST'>
        <input type='hidden' name='Continue' value='{0}' />
        <dl>
            <dt>User Name:</dt>
            <dd><input type='text' name='UserName' /></dd>
            <dt>Password:</dt>
            <dd><input type='password' name='Password' /></dd>
        </dl>             
        <input type='submit' value='Sign In' />
    </form>
</body>
</html>";

        [AddHeader(ContentType = MimeTypes.Html)] //HTML is ASP.NET's default, but nice to be explicit
        public object Any(Login request)
        {
            return HtmlBody.Fmt(request.Redirect);
        }
#else
        //
        public object Any(Login request)
        {
            return HttpResult.Redirect("/");
        }
#endif
    }
}