using System;
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
		IEnumerable<TwitterUser> DownloadUsersByIds(IEnumerable<string> userIds);
		IEnumerable<TwitterUser> DownloadTwitterUsersByNames(IEnumerable<string> screenNames);
		List<TwitterUser> GetFriends(ulong userId, int skip = 0, int? take = null);
		List<TwitterUser> GetFriends(string screenName, int skip = 0, int? take = null);
		List<TwitterUser> GetFollowers(ulong userId, int skip = 0, int? take = null);
		List<TwitterUser> GetFollowers(string screenName, int skip = 0, int? take = null);
		List<Tweet> GetTweets(string screenName, string sinceId = null, int? take = null);
	}

	public class TwitterGateway : ITwitterGateway
	{
		public const int DefaultTake = 100;
		public const int ApiBatchSize = 100;
		public const string UserUrl      = "http://api.twitter.com/1/users/lookup.json";
		public const string FollowersUrl = "http://api.twitter.com/1/followers/ids.json";
		public const string FriendsUrl   = "http://api.twitter.com/1/friends/ids.json";
		public const string TweetsUrl    = "http://api.twitter.com/1/statuses/following_timeline.json?include_entities=1&screen_name={0}";

		public IEnumerable<TwitterUser> DownloadUsersByIds(IEnumerable<string> userIds)
		{
			var results = new List<TwitterUser>();
			var urls = userIds.BatchesOf(ApiBatchSize).ToList()
				.ConvertAll(x => UserUrl.AddQueryParam("user_id", string.Join(",", x.ToList())));

			var tasks = urls.DownloadAllJsonAsync();
			Task.WaitAll(tasks.ToArray());
			results.AddRange(tasks.SelectMany(x => x.Result.FromJson<List<TwitterUser>>()));

			return results;
		}

		public IEnumerable<TwitterUser> DownloadTwitterUsersByNames(IEnumerable<string> screenNames)
		{
			var results = new List<TwitterUser>();

			var urls = screenNames.BatchesOf(ApiBatchSize).ToList()
				.ConvertAll(x => UserUrl.AddQueryParam("screen_name", string.Join(",", x.ToList())));

			var tasks = urls.DownloadAllJsonAsync();
			Task.WaitAll(tasks.ToArray());
			results.AddRange(tasks.SelectMany(x => x.Result.FromJson<List<TwitterUser>>()));

			return results;
		}

		public List<TwitterUser> DownloadUsersFromUrl(string url, int skip = 0, int? take = null)
		{
			try
			{
				var json = url.DownloadJsonFromUrl();
				var userIds = JsonObject.Parse(json).JsonTo<List<string>>("ids");
				var requestedUserIds = userIds.Skip(skip).Take(take.GetValueOrDefault(DefaultTake));
				return DownloadUsersByIds(requestedUserIds).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}

		public List<TwitterUser> GetFriends(ulong userId, int skip = 0, int? take = null)
		{
			return DownloadUsersFromUrl(
				FriendsUrl.AddQueryParam("user_id", userId.ToString()), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<TwitterUser> GetFriends(string screenName, int skip = 0, int? take = null)
		{
			return DownloadUsersFromUrl(
				FriendsUrl.AddQueryParam("screen_name", screenName), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<TwitterUser> GetFollowers(ulong userId, int skip = 0, int? take = null)
		{
			return DownloadUsersFromUrl(
				FollowersUrl.AddQueryParam("user_id", userId.ToString()), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<TwitterUser> GetFollowers(string screenName, int skip = 0, int? take = null)
		{
			return DownloadUsersFromUrl(
				FollowersUrl.AddQueryParam("screen_name", screenName), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<Tweet> GetTweets(string screenName, string sinceId = null, int? take = null)
		{
			if (screenName == null)
				throw new ArgumentNullException("screenName");

			var url =  TweetsUrl.Fmt(screenName).AddQueryParam("count", take.GetValueOrDefault(DefaultTake).ToString());
			if (!string.IsNullOrEmpty(sinceId))
				url = url.AddQueryParam("max_id", sinceId);

			var json = url.DownloadJsonFromUrl();
			var tweets = json.FromJson<List<Tweet>>();

			return tweets;
		}
	}
}