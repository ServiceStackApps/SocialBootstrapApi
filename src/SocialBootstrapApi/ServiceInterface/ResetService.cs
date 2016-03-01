using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Web;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
    [Route("/reset")]
    public class Reset
    {
        public string Name { get; set; }
    }

    public class ResetResponse
    {
        public string Result { get; set; }
    }

    public class ResetService : Service
    {
        //public 
        object Any(Reset request)
        {
            Db.DeleteAll<User>();
            Db.DeleteAll<UserAuth>();
            Db.DeleteAll<UserAuthDetails>();

            var httpRes = (IHttpResponse)base.Request.Response;
            httpRes.Cookies.DeleteCookie("ss-id");
            httpRes.Cookies.DeleteCookie("ss-pid");

            return new ResetResponse { Result = "OK " + request.Name };
        }
    }
}