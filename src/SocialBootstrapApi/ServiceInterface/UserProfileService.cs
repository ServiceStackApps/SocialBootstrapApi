using System.Linq;
using ServiceStack.Authentication.OpenId;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
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
        public string GoogleUserId { get; set; }
        public string GoogleFullName { get; set; }
        public string GoogleEmail { get; set; }
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

			var userProfile = session.TranslateTo<UserProfile>();
			userProfile.Id = int.Parse(session.UserAuthId);

            var user = Db.QueryById<User>(userProfile.Id);
			userProfile.PopulateWith(user);

            var userAuths = Db.Select<UserOAuthProvider>("UserAuthId = {0}", session.UserAuthId.ToInt());

		    var googleAuth = userAuths.FirstOrDefault(x => x.Provider == GoogleOpenIdOAuthProvider.Name);
            if (googleAuth != null)
            {
                userProfile.GoogleUserId = googleAuth.UserId;
                userProfile.GoogleFullName = googleAuth.FullName;
                userProfile.GoogleEmail = googleAuth.Email;
            }

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