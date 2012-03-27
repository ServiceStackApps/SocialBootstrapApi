using ServiceStack.CacheAccess;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;

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
			DbFactory.Exec(dbCmd => {
				dbCmd.DeleteAll<UserAuth>();
				dbCmd.DeleteAll<UserOAuthProvider>();
			});

			var httpRes = base.RequestContext.Get<IHttpResponse>();
			httpRes.Cookies.DeleteCookie("ss-id");
			httpRes.Cookies.DeleteCookie("ss-pid");
			
			return new ResetResponse { Result = "OK " + request.Name };
		}
	}
}