using ServiceStack.CacheAccess;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	[RestService("/reset")]
	public class Reset
	{
		public string Name { get; set; }
	}

	public class ResetResponse : IHasResponseStatus
	{
		public string Result { get; set; }
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class ResetService : ServiceBase<Reset>
	{
		public IDbConnectionFactory DbFactory { get; set; }

		protected override object Run(Reset request)
		{
		    using (var db = DbFactory.OpenDbConnection())
		    {
                db.DeleteAll<User>();
                db.DeleteAll<UserAuth>();
                db.DeleteAll<UserOAuthProvider>();
            }

			var httpRes = base.RequestContext.Get<IHttpResponse>();
			httpRes.Cookies.DeleteCookie("ss-id");
			httpRes.Cookies.DeleteCookie("ss-pid");
			
			return new ResetResponse { Result = "OK " + request.Name };
		}
	}
}