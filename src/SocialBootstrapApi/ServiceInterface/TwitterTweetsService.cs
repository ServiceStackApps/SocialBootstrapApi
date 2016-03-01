using System.Collections.Generic;
using ChaweetApi.ServiceModel;
using ServiceStack;

namespace SocialBootstrapApi.ServiceInterface
{
    public class TwitterTweets
    {
        public string ScreenName { get; set; }

        public int? Take { get; set; }

        public string SinceId { get; set; }

        public string MaxId { get; set; }
    }

    public class TwitterTweetsResponse
    {
        public List<Tweet> Results { get; set; }
    }

    public class TwitterTweetsService : AppServiceBase
    {
        public object Any(TwitterTweets request)
        {
            var cacheKey = "cache:Tweet:" + request.ScreenName + ":tweets"
                + (request.Take.HasValue ? ":take:" + request.Take : "")
                + (!request.SinceId.IsNullOrEmpty() ? ":sinceid:" + request.SinceId : "")
                + (!request.MaxId.IsNullOrEmpty() ? ":maxid:" + request.MaxId : "");

            //This caches and returns the most optimal result the browser can handle, e.g.
            //If the browser requests json and accepts deflate - it returns a deflated json payload from cache
            return base.Request.ToOptimizedResultUsingCache(Cache, cacheKey, () =>
                new TwitterTweetsResponse
                {
                    Results = AuthTwitterGateway.GetTweets(
                        request.ScreenName, request.SinceId, request.MaxId, request.Take)
                });
        }
    }

}