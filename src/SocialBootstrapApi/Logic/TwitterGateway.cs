using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ChaweetApi.ServiceModel;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;
using SocialBootstrapApi.Support;

namespace SocialBootstrapApi.Logic
{
	public interface ITwitterGateway
	{
		ITwitterGateway CreateAuthroizedGateway(OAuthProvider authProvider, string accessToken, string accessTokenSecret);

		IEnumerable<TwitterUser> DownloadUsersByIds(IEnumerable<string> userIds);
		IEnumerable<TwitterUser> DownloadTwitterUsersByNames(IEnumerable<string> screenNames);
		List<TwitterUser> GetFriends(ulong userId, int skip = 0, int? take = null);
		List<TwitterUser> GetFriends(string screenName, int skip = 0, int? take = null);
		List<TwitterUser> GetFollowers(ulong userId, int skip = 0, int? take = null);
		List<TwitterUser> GetFollowers(string screenName, int skip = 0, int? take = null);
		List<Tweet> GetTweets(string screenName, string sinceId = null, string maxId = null, int? take = null);
		List<Tweet> GetTimeline(string screenName, string sinceId = null, string maxId = null, int? take = null);
		List<DirectMessage> GetDirectMessages(string sinceId = null, string maxId = null, int? take = null);
	}

	public class TwitterGateway : ITwitterGateway
	{
		public const int DefaultTake  = 100;
		public const int ApiBatchSize = 100;
		public const string UserUrl           = "http://api.twitter.com/1/users/lookup.json";
		public const string FollowersUrl      = "http://api.twitter.com/1/followers/ids.json";
		public const string FriendsUrl        = "http://api.twitter.com/1/friends/ids.json";
		public const string UserTimelineUrl   = "http://api.twitter.com/1/statuses/user_timeline.json?include_entities=1&screen_name={0}";
		public const string TweetsUrl         = "http://api.twitter.com/1/statuses/following_timeline.json?include_entities=1&screen_name={0}";
		public const string DirectMessagesUrl = "https://api.twitter.com/1/direct_messages.json";

		public TwitterAuth Auth { get; set; }

		public ITwitterGateway CreateAuthroizedGateway(OAuthProvider authProvider, string accessToken, string accessTokenSecret)
		{
			return new TwitterGateway {
				Auth = new TwitterAuth {
					OAuthProvider = authProvider,
					AccessToken = accessToken,
					AccessTokenSecret = accessTokenSecret,
				}
			};
		}

		public IEnumerable<TwitterUser> DownloadUsersByIds(IEnumerable<string> userIds)
		{
			var results = new List<TwitterUser>();
			var urls = userIds.BatchesOf(ApiBatchSize).ToList()
				.ConvertAll(x => UserUrl.AddQueryParam("user_id", string.Join(",", x.ToList())));

			var tasks = urls.DownloadAllJsonAsync(Auth);
			Task.WaitAll(tasks.ToArray());
			results.AddRange(tasks.SelectMany(x => x.Result.FromJson<List<TwitterUser>>()));

			return results;
		}

		public IEnumerable<TwitterUser> DownloadTwitterUsersByNames(IEnumerable<string> screenNames)
		{
			var results = new List<TwitterUser>();

			var urls = screenNames.BatchesOf(ApiBatchSize).ToList()
				.ConvertAll(x => UserUrl.AddQueryParam("screen_name", string.Join(",", x.ToList())));

			var tasks = urls.DownloadAllJsonAsync(Auth);
			Task.WaitAll(tasks.ToArray());
			results.AddRange(tasks.SelectMany(x => x.Result.FromJson<List<TwitterUser>>()));

			return results;
		}

		public List<TwitterUser> DownloadUsersFromUrl(string url, int skip = 0, int? take = null)
		{
			try
			{
				var json = url.DownloadJsonFromUrl(Auth);
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
				FriendsUrl.AddQueryParam("user_id", userId), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<TwitterUser> GetFriends(string screenName, int skip = 0, int? take = null)
		{
			return DownloadUsersFromUrl(
				FriendsUrl.AddQueryParam("screen_name", screenName), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<TwitterUser> GetFollowers(ulong userId, int skip = 0, int? take = null)
		{
			return DownloadUsersFromUrl(
				FollowersUrl.AddQueryParam("user_id", userId), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<TwitterUser> GetFollowers(string screenName, int skip = 0, int? take = null)
		{
			return DownloadUsersFromUrl(
				FollowersUrl.AddQueryParam("screen_name", screenName), skip, take.GetValueOrDefault(DefaultTake));
		}

		public List<Tweet> GetTweets(string screenName, string sinceId = null, string maxId = null, int? take = null)
		{
			if (screenName == null)
				throw new ArgumentNullException("screenName");

			var url =  TweetsUrl.Fmt(screenName).AddQueryParam("count", take.GetValueOrDefault(DefaultTake));
			if (!string.IsNullOrEmpty(sinceId))
				url = url.AddQueryParam("since_id", sinceId);
			if (!string.IsNullOrEmpty(maxId))
				url = url.AddQueryParam("max_id", maxId);

			var json = url.DownloadJsonFromUrl(Auth);
			var tweets = json.FromJson<List<Tweet>>();

			return tweets;
		}

		public List<Tweet> GetTimeline(string screenName, string sinceId = null, string maxId = null, int? take = null)
		{
			if (screenName == null)
				throw new ArgumentNullException("screenName");

			var url =  UserTimelineUrl.Fmt(screenName).AddQueryParam("count", take.GetValueOrDefault(DefaultTake));
			if (!string.IsNullOrEmpty(sinceId))
				url = url.AddQueryParam("since_id", sinceId);
			if (!string.IsNullOrEmpty(maxId))
				url = url.AddQueryParam("max_id", maxId);

			var json = url.DownloadJsonFromUrl(Auth);
			var tweets = json.FromJson<List<Tweet>>();

			return tweets;
		}


		public List<DirectMessage> GetDirectMessages(string sinceId = null, string maxId = null, int? take = null)
		{
			var url =  DirectMessagesUrl.AddQueryParam("count", take.GetValueOrDefault(DefaultTake));
			if (!string.IsNullOrEmpty(sinceId))
				url = url.AddQueryParam("since_id", sinceId);
			if (!string.IsNullOrEmpty(maxId))
				url = url.AddQueryParam("max_id", maxId);

			var json = url.DownloadJsonFromUrl(Auth);
			var tweets = json.FromJson<List<DirectMessage>>();

			return tweets;
		}


	}

	public class TwitterAuth
	{
		public OAuthProvider OAuthProvider { get; set; }
		public string AccessToken { get; set; }
		public string AccessTokenSecret { get; set; }
	}

	public static class TwitterGatewayExtensions
	{
		public static string DownloadJsonFromUrl(this string url, TwitterAuth twitterAuth)
		{
			if (twitterAuth == null)
				return url.DownloadJsonFromUrl();

			var uri = new Uri(url);
			var webReq = (HttpWebRequest)WebRequest.Create(uri);
			webReq.Accept = ContentType.Json;
			if (twitterAuth.AccessToken != null)
			{
				webReq.Headers[HttpRequestHeader.Authorization] = OAuthAuthorizer.AuthorizeRequest(
					twitterAuth.OAuthProvider, twitterAuth.AccessToken, twitterAuth.AccessTokenSecret, HttpMethods.Get, uri, null);
			}

			using (var webRes = webReq.GetResponse())
				return webRes.DownloadText();
		}

		public static List<Task<string>> DownloadAllJsonAsync(this IEnumerable<string> urls, TwitterAuth twitterAuth)
		{
			if (twitterAuth == null)
				return urls.DownloadAllAsync(ContentType.Json);

			return urls.DownloadAllAsync(ContentType.Json, (webReq, uri) => {

				if (twitterAuth.AccessToken != null)
				{
					webReq.Headers[HttpRequestHeader.Authorization] = OAuthAuthorizer.AuthorizeRequest(
                        twitterAuth.OAuthProvider, twitterAuth.AccessToken, twitterAuth.AccessTokenSecret, HttpMethods.Get, uri, null);
				}

			});
		}

	}

}