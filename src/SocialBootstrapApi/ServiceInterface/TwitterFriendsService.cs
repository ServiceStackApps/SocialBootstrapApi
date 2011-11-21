using System;
using System.Collections.Generic;
using ChaweetApi.ServiceModel;
using ServiceStack.CacheAccess;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.Logic;

namespace SocialBootstrapApi.ServiceInterface
{
	//Request DTO
	public class TwitterFriends
	{
		public string UserId { get; set; }
		public string ScreenName { get; set; }
	}

	//Response DTO
	public class TwitterFriendsResponse : IHasResponseStatus
	{
		public TwitterFriendsResponse()
		{
			this.Results = new List<TwitterUser>();
			this.ResponseStatus = new ResponseStatus();
		}

		public List<TwitterUser> Results { get; set; }

		//DTO to hold error messages and Exceptions get auto-serialized into
		public ResponseStatus ResponseStatus { get; set; } 
	}

	public class TwitterFriendsService : ServiceBase<TwitterFriends>
	{
		public ITwitterGateway TwitterGateway { get; set; } //Injected in IOC as defined in AppHost
		
		public ICacheClient Cache { get; set; } //Injected in IOC as defined in AppHost

		//Available on all HTTP Verbs (GET, POST, PUT, DELETE, etc) and endpoints JSON, XMl, JSV, etc
		protected override object Run(TwitterFriends request)
		{
			if (request.UserId.IsNullOrEmpty() && request.ScreenName.IsNullOrEmpty())
				throw new ArgumentNullException("UserId or UserName is required");

			var hasId = !request.UserId.IsNullOrEmpty();

			//Create a unique cache key for this request
			var cacheKey = "urn:User:" + (hasId ? "Id" : "Name") + ":friends";

			//This caches and returns the most optimal result the browser can handle, e.g.
			//If the browser requests json and accepts deflate - it returns a deflated json payload from cache
			return base.RequestContext.ToOptimizedResultUsingCache(Cache, cacheKey, () =>
				new TwitterFriendsResponse {
					Results = hasId
						? TwitterGateway.GetFriends(ulong.Parse(request.UserId))
						: TwitterGateway.GetFriends(request.ScreenName)
				});
		}
	}

}