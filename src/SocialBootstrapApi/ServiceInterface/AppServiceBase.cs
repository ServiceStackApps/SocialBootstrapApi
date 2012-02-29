using System.Linq;
using ServiceStack.CacheAccess;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SocialBootstrapApi.Logic;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	public abstract class AppServiceBase<T> : ServiceBase<T>
	{
		public ICacheClient Cache { get; set; } //Injected in IOC as defined in AppHost

		private CustomUserSession userSession;
		protected CustomUserSession UserSession
		{
			get
			{
				if (userSession != null) return userSession;
				if (SessionKey != null)
					userSession = this.Cache.Get<CustomUserSession>(SessionKey);
				else
					SessionFeature.CreateSessionIds();

				var unAuthorizedSession = new CustomUserSession();
				return userSession ?? (userSession = unAuthorizedSession);
			}
		}

		protected string SessionKey
		{
			get
			{
				var sessionId = SessionFeature.GetSessionId();
				return sessionId == null ? null : SessionFeature.GetSessionKey(sessionId);
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