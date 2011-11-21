using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChaweetApi.ServiceModel;
using ServiceStack.Common;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using SocialBootstrapApi.Support;

namespace SocialBootstrapApi.Logic
{
	public interface ITwitterGateway 
	{
		IEnumerable<TwitterUser> DownloadTwitterUsersByIds(IEnumerable<string> userIds);
		IEnumerable<TwitterUser> DownloadTwitterUsersByNames(IEnumerable<string> screenNames);
		List<TwitterUser> GetFriends(ulong userId);
		List<TwitterUser> GetFriends(string screenName);
		List<TwitterUser> GetFollowers(ulong userId);
		List<TwitterUser> GetFollowers(string screenName);
	}

	public class TwitterGateway : ITwitterGateway
	{
		public const int ApiBatchSize = 100;
		public const string UserUrl      = "http://api.twitter.com/1/users/lookup.json";
		public const string FollowersUrl = "http://api.twitter.com/1/followers/ids.json";
		public const string FriendsUrl   = "http://api.twitter.com/1/friends/ids.json";
		
		public IEnumerable<TwitterUser> DownloadTwitterUsersByIds(IEnumerable<string> userIds)
		{
			var results = new List<TwitterUser>();
			foreach (var userIdsBatch in userIds.BatchesOf(ApiBatchSize))
			{
				var urls = userIdsBatch.ToList().ConvertAll(x => 
					UserUrl.AddQueryParam("user_id", x));

				var tasks = urls.DownloadAllJsonAsync();
				Task.WaitAll(tasks.ToArray());
				results.AddRange(tasks.SelectMany(x => x.Result.FromJson<List<TwitterUser>>()));
			}
			return results;
		}

		public IEnumerable<TwitterUser> DownloadTwitterUsersByNames(IEnumerable<string> screenNames)
		{
			var results = new List<TwitterUser>();

			foreach (var screenNamesBatch in screenNames.BatchesOf(ApiBatchSize))
			{
				var urls = screenNamesBatch.ToList().ConvertAll(x => UserUrl.AddQueryParam("screen_name", x));
				var tasks = urls.DownloadAllJsonAsync();
				Task.WaitAll(tasks.ToArray());
				results.AddRange(tasks.SelectMany(x => x.Result.FromJson<List<TwitterUser>>()));
			}

			return results;
		}

		public List<TwitterUser> GetFriends(ulong userId)
		{
			var json = FriendsUrl.AddQueryParam("user_id", userId.ToString()).DownloadJsonFromUrl();
			return DownloadTwitterUsersByIds(json.FromJson<List<string>>()).ToList();
		}

		public List<TwitterUser> GetFriends(string screenName)
		{
			var json = FriendsUrl.AddQueryParam("screen_name", screenName).DownloadJsonFromUrl();
			return DownloadTwitterUsersByNames(json.FromJson<List<string>>()).ToList();
		}

		public List<TwitterUser> GetFollowers(ulong userId)
		{
			var json = FollowersUrl.AddQueryParam("user_id", userId.ToString()).DownloadJsonFromUrl();
			return DownloadTwitterUsersByIds(json.FromJson<List<string>>()).ToList();
		}

		public List<TwitterUser> GetFollowers(string screenName)
		{
			var json = FollowersUrl.AddQueryParam("screen_name", screenName).DownloadJsonFromUrl();
			return DownloadTwitterUsersByNames(json.FromJson<List<string>>()).ToList();
		}
	}
}