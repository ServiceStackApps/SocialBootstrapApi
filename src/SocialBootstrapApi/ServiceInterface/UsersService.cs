using System.Collections.Generic;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.App_Start;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	public class Users
	{
		public int[] UserIds { get; set; }
	}

	public class UsersResponse : IHasResponseStatus
	{
		public UsersResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		public List<UserInfo> Results { get; set; }
	
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class UsersService : RestServiceBase<Users>
	{
		public IDbConnectionFactory DbFactory { get; set; }

		public override object OnGet(Users request)
		{
			var response = new UsersResponse();
			
			if (request.UserIds.Length == 0)
				return response;
			
			var users = DbFactory.Exec(dbCmd => dbCmd.GetByIds<User>(request.UserIds));

			var userInfos = users.ConvertAll(x => x.TranslateTo<UserInfo>());

			return new UsersResponse {
				Results = userInfos
			};
		}
	}

	public class UserInfo
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string DisplayName { get; set; }
		public string TwitterUserId { get; set; }
		public string TwitterScreenName { get; set; }
		public string FacebookUserId { get; set; }
		public string ProfileImageUrl64 { get; set; }
	}
}