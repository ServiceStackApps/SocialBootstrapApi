using System.Collections.Generic;
using ChaweetApi.ServiceModel;
using ServiceStack.Common;
using ServiceStack.ServiceHost;

namespace SocialBootstrapApi.ServiceInterface
{
	public class TwitterTimelines
	{
		public string ScreenName { get; set; }

		public int? Take { get; set; }

		public string SinceId { get; set; }

		public string MaxId { get; set; }
	}

	public class TwitterTimelinesResponse
	{
		public List<Tweet> Results { get; set; }
	}

	public class TwitterTimelinesService : AppServiceBase
	{
	    public object Any(TwitterTimelines request)
		{
			var cacheKey = "cache:Tweet:" + request.ScreenName + ":timeline"
				+ (request.Take.HasValue ? ":take:" + request.Take : "")
				+ (!request.SinceId.IsNullOrEmpty() ? ":sinceid:" + request.SinceId : "")
				+ (!request.MaxId.IsNullOrEmpty() ? ":maxid:" + request.MaxId : "");

			//This caches and returns the most optimal result the browser can handle, e.g.
			//If the browser requests json and accepts deflate - it returns a deflated json payload from cache
			return base.RequestContext.ToOptimizedResultUsingCache(Cache, cacheKey, () =>
				new TwitterTimelinesResponse {
					Results = AuthTwitterGateway.GetTimeline(
						request.ScreenName, request.SinceId, request.MaxId, request.Take)
				});
		}
	}

}