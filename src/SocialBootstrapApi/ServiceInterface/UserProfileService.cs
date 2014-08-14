using System.Linq;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Authentication.OpenId;
using ServiceStack.OrmLite;
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
        public string YahooUserId { get; set; }
        public string YahooFullName { get; set; }
        public string YahooEmail { get; set; }
        public string GravatarImageUrl64 { get; set; }
    }

	public class UserProfileResponse
	{
		public UserProfile Result { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
	}

    [Authenticate]
    public class UserProfileService : AppServiceBase
	{
		public object Get(UserProfile request)
		{
		    var session = base.UserSession;

			var userProfile = session.ConvertTo<UserProfile>();
			userProfile.Id = int.Parse(session.UserAuthId);

            var user = Db.SingleById<User>(userProfile.Id);
			userProfile.PopulateWith(user);

            var userAuths = Db.Select<UserAuthDetails>(q => q.UserAuthId == session.UserAuthId.ToInt());

            var yahooAuth = userAuths.FirstOrDefault(x => x.Provider == YahooOpenIdOAuthProvider.Name);
            if (yahooAuth != null)
            {
                userProfile.YahooUserId = yahooAuth.UserId;
                userProfile.YahooFullName = yahooAuth.FullName;
                userProfile.YahooEmail = yahooAuth.Email;
            }

			return new UserProfileResponse {
				Result = userProfile
			};
		}
	}

}