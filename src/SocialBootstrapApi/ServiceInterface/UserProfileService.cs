using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
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

	public class UserProfileResponse
	{
		public UserProfile Result { get; set; }
	}

    [Authenticate]
    public class UserProfileService : AppServiceBase
	{
		public object Get(UserProfile request)
		{
		    var session = base.UserSession;

			var userProfile = session.TranslateTo<UserProfile>();
			userProfile.Id = int.Parse(session.UserAuthId);

            var user = Db.QueryById<User>(userProfile.Id);
			userProfile.PopulateWith(user);

			return new UserProfileResponse {
				Result = userProfile
			};
		}
	}

}