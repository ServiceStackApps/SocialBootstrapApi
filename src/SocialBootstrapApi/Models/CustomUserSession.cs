using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;

namespace SocialBootstrapApi.Models
{
	/// <summary>
	/// Create your own strong-typed Custom AuthUserSession where you can add additional AuthUserSession 
	/// fields required for your application. The base class is automatically populated with 
	/// User Data as and when they authenticate with your application. 
	/// </summary>
	public class CustomUserSession : AuthUserSession
	{
		public string CustomId { get; set; }

		public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
		{
			base.OnAuthenticated(authService, session, tokens, authInfo);

			//Populate all matching fields from this session to your own custom User table
			var user = session.TranslateTo<User>();
			user.Id = int.Parse(session.UserAuthId);
			user.GravatarImageUrl64 = !session.Email.IsNullOrEmpty()
				? CreateGravatarUrl(session.Email, 64)
				: null;

			foreach (var authToken in session.ProviderOAuthAccess)
			{
				if (authToken.Provider == FacebookAuthProvider.Name)
				{
					user.FacebookName = authToken.DisplayName;
					user.FacebookFirstName = authToken.FirstName;
					user.FacebookLastName = authToken.LastName;
					user.FacebookEmail = authToken.Email;
				}
				else if (authToken.Provider == TwitterAuthProvider.Name)
				{
					user.TwitterName = authToken.DisplayName;
				}
			}

			//Resolve the DbFactory from the IOC and persist the user info
			authService.TryResolve<IDbConnectionFactory>().Exec(dbCmd => dbCmd.Save(user));
		}

		private static string CreateGravatarUrl(string email, int size = 64)
		{
			var md5 = MD5.Create();
			var md5HadhBytes = md5.ComputeHash(email.ToUtf8Bytes());

			var sb = new StringBuilder();
			for (var i = 0; i < md5HadhBytes.Length; i++)
				sb.Append(md5HadhBytes[i].ToString("x2"));

			string gravatarUrl = "http://www.gravatar.com/avatar/{0}?d=mm&s={1}".Fmt(sb, size);
			return gravatarUrl;
		}		
	}
}