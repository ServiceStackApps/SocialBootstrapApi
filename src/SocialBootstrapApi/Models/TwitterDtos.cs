using System;
using System.Collections.Generic;

namespace ChaweetApi.ServiceModel
{
	public class GeoPoint
	{
        public string type { get; set; }
        public double[] coordinates { get; set; }
	}

	public class SizeEntity
	{
        public int w { get; set; }
        public int h { get; set; }
        public string resize { get; set; }
	}

	public class MediaEntity
	{
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public Dictionary<string,SizeEntity> sizes { get; set; }
        public string type  { get; set; }
        public int[] indices { get; set; }
	}

	public class UrlEntity
	{
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public int[] indices { get; set; }
	}

	public class UserMentionEntity
	{
        public ulong id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public int[] indices { get; set; }
	}

	public class HashTagEntity
	{
        public string text { get; set; }
        public int[] indicies { get; set; }
	}

	public class TweetEntities
	{
		public MediaEntity[] media { get; set; }
        public UrlEntity[] urls { get; set; }
        public UserMentionEntity[] user_mentions { get; set; }
        public HashTagEntity[] hashtags { get; set; }
	}

	public class UserStat
	{
        public ulong id { get; set; }
		public string screen_name { get; set; }
        public string name { get; set; }
        public int friends_count { get; set; }
        public int followers_count { get; set; }
        public int listed_count { get; set; }
        public int statuses_count { get; set; } 
	}

	public class Tweet
	{
		public ulong id { get; set; }
		public ulong? in_reply_to_status_id { get; set; }
		public bool retweeted { get; set; }
		public bool truncated { get; set; }
		public string created_at { get; set; }
		public ulong? in_reply_to_user_id { get; set; }
		public string in_reply_to_screen_name { get; set; }
		public TweetUser user { get; set; }
		public TweetEntities entities { get; set; }
		public bool favorited { get; set; }
		public string source { get; set; }
		public string retweet_count { get; set; }
		public string text { get; set; }
		public GeoPoint geo { get; set; }
		public GeoPoint coordinates { get; set; }
	}

	public class DirectMessage
	{
		public ulong id { get; set; }
		public string created_at { get; set; }
		public string sender_screen_name { get; set; }
		public TweetUser sender { get; set; }
		public TweetUser recipient { get; set; }
		public ulong recipient_id { get; set; }
		public ulong sender_id { get; set; }
		public string recipient_screen_name { get; set; }
		public string text { get; set; }
	}
	
	public class TweetUser
	{
		public string name { get; set; }
		public string profile_sidebar_border_color { get; set; }
		public string profile_background_tile { get; set; }
		public string profile_sidebar_fill_color { get; set; }
		public string created_at { get; set; }
		public string profile_image_url { get; set; }
		public string profile_link_color { get; set; }
		public string location { get; set; }
		public string url { get; set; }
		public int favourites_count { get; set; }
		public bool contributors_enabled { get; set; }
		public string utc_offset { get; set; }
		public string id { get; set; }
		public string profile_use_background_image { get; set; }
		public string profile_text_color { get; set; }
		public bool @protected { get; set; }
		public int followers_count { get; set; }
		public string lang { get; set; }
		public bool verified { get; set; }
		public string profile_background_color { get; set; }
		public bool geo_enabled { get; set; }
		public bool? notifications { get; set; }
		public string description { get; set; }
		public string time_zone { get; set; }
		public int friends_count { get; set; }
		public int statuses_count { get; set; }
		public string profile_background_image_url { get; set; }
		public string screen_name { get; set; }
	}
	
	public class TwitterUser
	{
		public string name { get; set; }
		public string profile_sidebar_border_color { get; set; }
		public string profile_background_tile { get; set; }
		public string profile_sidebar_fill_color { get; set; }
		public string created_at { get; set; }
		public string profile_image_url { get; set; }
		public string profile_link_color { get; set; }
		public bool? follow_request_sent { get; set; }
		public string location { get; set; }
		public string url { get; set; }
		public int favourites_count { get; set; }
		public bool contributors_enabled { get; set; }
		public string utc_offset { get; set; }
		public string id { get; set; }
		public string profile_use_background_image { get; set; }
		public string profile_text_color { get; set; }
		public bool @protected { get; set; }
		public int followers_count { get; set; }
		public string lang { get; set; }
		public bool verified { get; set; }
		public string profile_background_color { get; set; }
		public bool geo_enabled { get; set; }
		public bool? notifications { get; set; }
		public string description { get; set; }
		public string time_zone { get; set; }
		public int friends_count { get; set; }
		public int statuses_count { get; set; }
		public string profile_background_image_url { get; set; }
		public Tweet status { get; set; }
		public string screen_name { get; set; }
		public bool? following { get; set; }
	}

	public class TwitterUserIds
	{
		public ulong next_cursor { get; set; }
		public ulong previous_cursor { get; set; }
		public ulong[] ids { get; set; }
		public string next_cursor_str { get; set; }
		public string previous_cursor_str { get; set; }
	}

}

