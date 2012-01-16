using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	[RestService("/userauths")]
	public class UserAuths
	{
		public int[] Ids { get; set; }
	}

	public class UserAuthsResponse : IHasResponseStatus
	{
		public UserAuthsResponse()
		{
			this.Users = new List<User>();
			this.UserAuths = new List<UserAuth>();
			this.OAuthProviders = new List<UserOAuthProvider>();
		}
		public List<User> Users { get; set; }

		public List<UserAuth> UserAuths { get; set; }

		public List<UserOAuthProvider> OAuthProviders { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	//Implementation. Can be called via any endpoint or format, see: http://servicestack.net/ServiceStack.Hello/
	public class UserAuthsService : ServiceBase<UserAuths>
	{
		public IDbConnectionFactory DbFactory { get; set; }

		protected override object Run(UserAuths request)
		{
			return new UserAuthsResponse {
				Users = DbFactory.Exec(dbCmd => dbCmd.Select<User>()),
				UserAuths = DbFactory.Exec(dbCmd => dbCmd.Select<UserAuth>()),
				OAuthProviders = DbFactory.Exec(dbCmd => dbCmd.Select<UserOAuthProvider>()),
			};
		}
	}
}