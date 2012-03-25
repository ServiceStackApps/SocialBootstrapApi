(function (root)
{
    if (!$("#twitter")[0]) return;

    var app = root.App,
        userProfile = app.models.userProfile,
        login = app.models.login,
        twitter = app.models.twitter;

    app.Routes = Backbone.Router.extend({
        routes: {
            "timelines": "timelines",
            "tweets": "tweets",
            "friends": "friends",
            "followers": "followers",
            ":user/timelines": "userTimelines",
            ":user/tweets": "userTweets",
            ":user/friends": "userFriends",
            ":user/followers": "userFollowers",
            "*catchall": "catchAll"
        },
        initialize: function(opt) {
            this.app = opt.app;
        },
        timelines: function() {
            this.app.route("twitterProfileChange", login.get('screenName'), "timelines");
        },
        tweets: function() {
            this.app.route("twitterProfileChange", login.get('screenName'), "tweets");
        },
        friends: function() {
            this.app.route("twitterProfileChange", login.get('screenName'), "friends");
        },
        followers: function() {
            this.app.route("twitterProfileChange", login.get('screenName'), "followers");
        },
        userTimelines: function(user) {
            this.app.route("twitterProfileChange", user, "timelines");
        },
        userTweets: function(user) {
            this.app.route("twitterProfileChange", user, "tweets");
        },
        userFriends: function(user) {
            this.app.route("twitterProfileChange", user, "friends");
        },
        userFollowers: function(user) {
            this.app.route("twitterProfileChange", user, "followers");
        },
        catchAll: function () {
            console.log("this is a default route");
            this.timelines();
        }
    });
    app.routes = new app.Routes({ app: app });

    userProfile.bind("change", function(profile) {
        if (profile.hasTwitterAuth()) {
            twitter.twitterProfileChange(twitter.get("screenName") || profile.get("twitterScreenName"));
        }
        login.set({ displayName: profile.get('displayName') });
    });

    login.bind("change:isAuthenticated", function() {
        if (app.isAuth())
            userProfile.fetch();
        else
            userProfile.clear();
    });

})(window);
