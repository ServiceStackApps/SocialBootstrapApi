using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
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
		public IDbConnectionFactory DbFactory { get; set; }

	    public object Any(Reset request)
		{
            Db.DeleteAll<User>();
            Db.DeleteAll<UserAuth>();
            Db.DeleteAll<UserOAuthProvider>();

			var httpRes = base.RequestContext.Get<IHttpResponse>();
			httpRes.Cookies.DeleteCookie("ss-id");
			httpRes.Cookies.DeleteCookie("ss-pid");
			
			return new ResetResponse { Result = "OK " + request.Name };
		}
	}
}