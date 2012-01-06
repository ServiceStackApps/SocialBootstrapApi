(function (root) {
    var app = root.App;
   
    app.Twitter = app.BaseModel.extend({
        defaults: {
            screenName: null,
            tab: "tweets",
            tweets: [],
            friends: [],
            followers: []
        },
        initialize: function (opt) {
            _.bindAll(this, "twitterProfileChange", "onChange", "twitterTab");

            var self = this;
            this.profile = opt.profile;
            this.profile.bind("change", function (profile) {
                self.twitterProfileChange(profile.get("twitterScreenName"));
            });
            this.bind("change", this.onChange);
        },
        onChange: function () {
            console.log("twitter.onChange:" + this.get('tab'));
            if (this.get('screenName')) 
                this.load(this.get('tab'));
            else 
                this.clear({silent:true});            
        },
        load: function(tab) {
            var self = this, o = {};
            _.get("api/twitter/" + this.get('screenName') + "/" + tab, function (r) {
                o[tab] = r.results;
                self.set(o);
            });
        },
        twitterProfileChange: function (screenName) {
            console.log("twitter.twitterProfileChange: " + this.get('tab'));
            this.set(_.defaults({ screenName: screenName }, this.defaults));
        },
        twitterTab: function (tab) {
            console.log("twitterTab:" + tab);
            this.set({ tab: tab });
        }
    });

    app.TwitterView = app.BaseView.extend({
        initialize: function () {
            _.bindAll(this, "render");
            this.model.bind("change", this.render);
            this.$el = $(this.el);
            this.$signedOut = this.$el.find(".signed-out");
            this.$signedIn = this.$el.find(".signed-in");
            this.$signedInBody = this.$el.find(".signed-in .tab-content");
            this.tweetsTemplate = _.template($("#template-tweets").html());
            this.usersTemplate = _.template($("#template-users").html());
        },
        render: function () {

            var screenName = this.model.get('screenName'),
                signedIn = !!screenName,
                html = "";

            console.log("twitter.render:" + screenName);

            this.$signedOut.toggle(!signedIn);
            this.$signedIn.toggle(signedIn);
            
            if (screenName) {
                var tab = this.model.get('tab');
                html = tab === "friends"
                    ? this.usersTemplate({ users: this.model.get('friends') })
                    : tab == "followers"
                        ? this.usersTemplate({ users: this.model.get('followers') })
                        : this.tweetsTemplate({ tweets: this.model.get('tweets') });
            }

            this.$signedInBody.html(html);
        }
    });

})(window);
