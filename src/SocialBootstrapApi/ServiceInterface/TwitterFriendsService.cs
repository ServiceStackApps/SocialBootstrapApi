using System;
using System.Collections.Generic;
using ChaweetApi.ServiceModel;
using ServiceStack;
using SocialBootstrapApi.Logic;

namespace SocialBootstrapApi.ServiceInterface
{
    //Request DTO
    public class TwitterFriends
    {
        public string UserId { get; set; }
        public string ScreenName { get; set; }
        public int Skip { get; set; }
    }

    //Response DTO
    public class TwitterFriendsResponse
    {
        public TwitterFriendsResponse()
        {
            this.Results = new List<TwitterUser>();
        }

        public List<TwitterUser> Results { get; set; }
    }

    public class TwitterFriendsService : AppServiceBase
    {
        //Available on all HTTP Verbs (GET, POST, PUT, DELETE, etc) and endpoints JSON, XMl, JSV, etc
        public object Any(TwitterFriends request)
        {
            if (request.UserId.IsNullOrEmpty() && request.ScreenName.IsNullOrEmpty())
                throw new ArgumentNullException("UserId or UserName is required");

            var hasId = !request.UserId.IsNullOrEmpty();

            //Create a unique cache key for this request
            var cacheKey = "cache:User:"
                + (hasId ? "Id:" + request.UserId : "Name:" + request.ScreenName)
                + ":skip:" + request.Skip
                + ":friends";

            //This caches and returns the most optimal result the browser can handle, e.g.
            //If the browser requests json and accepts deflate - it returns a deflated json payload from cache
            return base.Request.ToOptimizedResultUsingCache(Cache, cacheKey, () =>
                new TwitterFriendsResponse
                {
                    Results = hasId
                        ? AuthTwitterGateway.GetFriends(ulong.Parse(request.UserId), request.Skip)
                        : AuthTwitterGateway.GetFriends(request.ScreenName, request.Skip)
                });
        }
    }

}