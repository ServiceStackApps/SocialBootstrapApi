using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.OrmLite;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	[Route("/userauths")]
	public class UserAuths
	{
		public int[] Ids { get; set; }
	}

	public class UserAuthsResponse
	{
		public UserAuthsResponse()
		{
			this.Users = new List<User>();
			this.UserAuths = new List<UserAuth>();
			this.OAuthProviders = new List<UserAuthDetails>();
		}
		public CustomUserSession UserSession { get; set; }

		public List<User> Users { get; set; }

		public List<UserAuth> UserAuths { get; set; }

        public List<UserAuthDetails> OAuthProviders { get; set; }
	}

	//Implementation. Can be called via any endpoint or format, see: http://servicestack.net/ServiceStack.Hello/
	public class UserAuthsService : AppServiceBase
	{
	    public object Any(UserAuths request)
		{
            var response = new UserAuthsResponse {
                UserSession = base.UserSession,
                Users = Db.Select<User>(),
                UserAuths = Db.Select<UserAuth>(),
                OAuthProviders = Db.Select<UserAuthDetails>(),
            };

            response.UserAuths.ForEach(x => x.PasswordHash = "[Redacted]");
            response.OAuthProviders.ForEach(x =>
                x.AccessToken = x.AccessTokenSecret = x.RequestTokenSecret = "[Redacted]");
            if (response.UserSession != null)
                response.UserSession.ProviderOAuthAccess.ForEach(x =>
                x.AccessToken = x.AccessTokenSecret = x.RequestTokenSecret = "[Redacted]");

            return response;
        }
	}
}