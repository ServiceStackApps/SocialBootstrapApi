using System.Linq;
using ServiceStack.CacheAccess;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SocialBootstrapApi.Logic;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	public abstract class AppServiceBase : Service
	{
	    public CustomUserSession UserSession
		{
			get
			{
				return SessionAs<CustomUserSession>();
			}
		}

		public ITwitterGateway TwitterGateway { get; set; } //Injected in IOC as defined in AppHost

		ITwitterGateway authGateway;
		public ITwitterGateway AuthTwitterGateway
		{
			get
			{
				if (authGateway != null) return authGateway;

				var authProvider = AuthService.AuthProviders.FirstOrDefault(x => x is TwitterAuthProvider) as TwitterAuthProvider;
				var oAuthTokens = UserSession.GetOAuthTokens(TwitterAuthProvider.Name);

				return authGateway = TwitterGateway.CreateAuthroizedGateway(
					authProvider,
					oAuthTokens != null ? oAuthTokens.AccessToken : null,
					oAuthTokens != null ? oAuthTokens.AccessTokenSecret : null);
			}
		} 

	}
}