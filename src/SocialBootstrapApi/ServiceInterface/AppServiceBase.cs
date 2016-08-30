using System.Linq;
using ServiceStack;
using ServiceStack.Auth;
using SocialBootstrapApi.Logic;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
    public abstract class AppServiceBase : Service
    {
        public CustomUserSession UserSession => SessionAs<CustomUserSession>();

        public ITwitterGateway TwitterGateway { get; set; } //Injected in IOC as defined in AppHost

        private ITwitterGateway authGateway;
        public ITwitterGateway AuthTwitterGateway
        {
            get
            {
                if (authGateway != null)
                    return authGateway;

                var authProvider = (TwitterAuthProvider)AuthenticateService.GetAuthProviders().First(x => x is TwitterAuthProvider);
                var oAuthTokens = UserSession.GetAuthTokens(TwitterAuthProvider.Name);
                return authGateway = TwitterGateway.CreateAuthroizedGateway(authProvider, oAuthTokens?.AccessToken, oAuthTokens?.AccessTokenSecret);
            }
        }
    }
}