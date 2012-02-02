using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	[Authenticate]
	public class UserProfile
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string DisplayName { get; set; }
		public string TwitterUserId { get; set; }
		public string TwitterScreenName { get; set; }
		public string TwitterName { get; set; }
		public string FacebookName { get; set; }
		public string FacebookFirstName { get; set; }
		public string FacebookLastName { get; set; }
		public string FacebookUserId { get; set; }
		public string FacebookUserName { get; set; }
		public string FacebookEmail { get; set; }
		public string GravatarImageUrl64 { get; set; }
	}

	public class UserProfileResponse : IHasResponseStatus
	{
		public UserProfileResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		public UserProfile Result { get; set; }
	
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class UserProfileService : RestServiceBase<UserProfile>
	{
		public IDbConnectionFactory DbFactory { get; set; }

		public override object OnGet(UserProfile request)
		{
			var session = this.GetSession();

			var userProfile = session.TranslateTo<UserProfile>();
			userProfile.Id = int.Parse(session.UserAuthId);

			var user = DbFactory.Exec(dbCmd => dbCmd.QueryById<User>(userProfile.Id));
			userProfile.PopulateWith(user);

			return new UserProfileResponse {
				Result = userProfile
			};
		}
	}

}