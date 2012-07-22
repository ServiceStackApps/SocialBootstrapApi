using System;
using System.Collections.Generic;
using ChaweetApi.ServiceModel;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;

namespace SocialBootstrapApi.ServiceInterface
{
	//Request DTO
	public class TwitterFollowers
	{
		public string UserId { get; set; }
		public string ScreenName { get; set; }
		public int Skip { get; set; }
	}

	//Response DTO
	public class TwitterFollowersResponse : IHasResponseStatus
	{
		public TwitterFollowersResponse()
		{
			this.Results = new List<TwitterUser>();
			this.ResponseStatus = new ResponseStatus();
		}

		public List<TwitterUser> Results { get; set; }

		//DTO to hold error messages and Exceptions get auto-serialized into
		public ResponseStatus ResponseStatus { get; set; } 
	}

	public class AppFollowersServiceBase : AppServiceBase<TwitterFollowers>
	{
		//Available on all HTTP Verbs (GET, POST, PUT, DELETE, etc) and endpoints JSON, XMl, JSV, etc
		protected override object Run(TwitterFollowers request)
		{
			if (request.UserId.IsNullOrEmpty() && request.ScreenName.IsNullOrEmpty())
				throw new ArgumentNullException("UserId or UserName is required");

			var hasId = !request.UserId.IsNullOrEmpty();

			//Create a unique cache key for this request
			var cacheKey = "cache:User:" 
				+ (hasId ? "Id:" + request.UserId : "Name:" + request.ScreenName) 
				+ ":skip:" + request.Skip
				+ ":followers";

			//This caches and returns the most optimal result the browser can handle, e.g.
			//If browser requests json and accepts deflate - it returns a deflated json payload from cache
			return base.RequestContext.ToOptimizedResultUsingCache(Cache, cacheKey, () =>
				new TwitterFollowersResponse {
					Results = hasId
						? TwitterGateway.GetFollowers(ulong.Parse(request.UserId), request.Skip)
						: TwitterGateway.GetFollowers(request.ScreenName, request.Skip)
				});
		}
	}

}