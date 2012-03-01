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
	public class TwitterDirectMessages
	{
		public int? Take { get; set; }

		public string SinceId { get; set; }
		
		public string MaxId { get; set; }
	}

	public class TwitterDirectMessagesResponse
	{
		public TwitterDirectMessagesResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		public List<DirectMessage> Results { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	public class TwitterDirectMessagesService : AppServiceBase<TwitterDirectMessages>
	{
		protected override object Run(TwitterDirectMessages request)
		{
			var cacheKey = "cache:DirectMessage:" + base.UserSession.TwitterScreenName + ":dms"
				+ (request.Take.HasValue ? ":take:" + request.Take : "")
				+ (!request.SinceId.IsNullOrEmpty() ? ":sinceid:" + request.SinceId : "")
				+ (!request.MaxId.IsNullOrEmpty() ? ":maxid:" + request.MaxId : "");

			//This caches and returns the most optimal result the browser can handle, e.g.
			//If the browser requests json and accepts deflate - it returns a deflated json payload from cache
			return base.RequestContext.ToOptimizedResultUsingCache(Cache, cacheKey, () =>
				new TwitterDirectMessagesResponse {
					Results = AuthTwitterGateway.GetDirectMessages(
						request.SinceId, request.MaxId, request.Take)
				});
		}
	}

}