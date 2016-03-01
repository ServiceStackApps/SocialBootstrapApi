using System.Collections.Generic;
using ChaweetApi.ServiceModel;

namespace SocialBootstrapApi.ServiceInterface
{
    //Request DTO
    public class TwitterUsers
    {
        public List<string> UserIds { get; set; }
        public List<string> ScreenNames { get; set; }
    }

    //Response DTO
    public class TwitterUsersResponse
    {
        public TwitterUsersResponse()
        {
            this.Results = new List<TwitterUser>();
        }

        public List<TwitterUser> Results { get; set; }
    }

    public class TwitterUsersService : AppServiceBase
    {
        //Available on all HTTP Verbs (GET,POST,PUT,DELETE,etc) and endpoints JSON,XMl,JSV,etc
        public object Any(TwitterUsers request)
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