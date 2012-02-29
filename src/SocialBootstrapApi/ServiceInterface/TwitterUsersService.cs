using System.Collections.Generic;
using ChaweetApi.ServiceModel;
using ServiceStack.CacheAccess;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.Logic;

namespace SocialBootstrapApi.ServiceInterface
{
	//Request DTO
	public class TwitterUsers
	{
		public List<string> UserIds { get; set; }
		public List<string> ScreenNames { get; set; }
	}

	//Response DTO
	public class TwitterUsersResponse : IHasResponseStatus
	{
		public TwitterUsersResponse()
		{
			this.Results = new List<TwitterUser>();
			this.ResponseStatus = new ResponseStatus();
		}

		public List<TwitterUser> Results { get; set; }

		//DTO to hold error messages and Exceptions get auto-serialized into
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class TwitterUsersService : AppServiceBase<TwitterUsers>
	{
		//Available on all HTTP Verbs (GET,POST,PUT,DELETE,etc) and endpoints JSON,XMl,JSV,etc
		protected override object Run(TwitterUsers request)
		{
			var userIds = request.UserIds ?? new List<string>();
			var userNames = request.ScreenNames ?? new List<string>();

			var results = new List<TwitterUser>();
			if (userIds.Count > 0)
				results.AddRange(AuthTwitterGateway.DownloadUsersByIds(userIds));
			if (userNames.Count > 0)
				results.AddRange(AuthTwitterGateway.DownloadTwitterUsersByNames(userNames));

			return new TwitterUsersResponse { Results = results };
		}
	}
}