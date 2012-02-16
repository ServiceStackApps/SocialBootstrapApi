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
                this.clear({ silent: true });
        },
        load: function (tab) {
            tab = tab || this.defaults.tab;
            var self = this, o = {};
            _.get("api/twitter/" + this.get('screenName') + "/" + tab, function (r) {
                o[tab] = r.results;
                self.set(o);
            });
        },
        twitterProfileChange: function (screenName, tab) {
            tab = tab || this.get('tab');
            console.log("twitter.twitterProfileChange: " + tab);

            if (!screenName) {
                this.set({ screenName: this.profile.get("twitterScreenName") });
                return;
            }

            var self = this;
            _.get("api/twitter/" + screenName, function (r) {
                var o = _.defaults({ screenName: screenName }, self.defaults);
                _.extend(o, r.results[0]);
                o.tab = tab;
                self.set(o);
                self.navigate(self.navUrl());
            });
        },
        viewingSelf: function() {
            return this.profile.get("twitterScreenName") === this.get("screenName");
        },
        navUrl: function() {
            return (this.viewingSelf() ? "" : this.get("screenName") + "/") + this.get("tab");
        },
        twitterTab: function (tab) {
            tab = tab || this.defaults.tab;            
            console.log("twitterTab:" + tab);
            this.set({ tab: tab });
            $(".tabs [href=#" + tab + "]").click();
            this.navigate(this.navUrl());
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
            this.currentUserTemplate = _.template($("#template-current-user").html());
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

                this.$(".current-user").html(this.currentUserTemplate(this.model.toJSON()));
                var bgImg = this.model.get('profile_background_image_url'),
                    bgColor = this.model.get('profile_background_color') || '',
                    bgRepeat = this.model.get('profile_background_tile') === "true" ? '' : 'no-repeat';
                var bgCss = bgImg ? '#' + bgColor + ' url(' + bgImg + ') ' + bgRepeat + ' top left' : '';
                $("BODY").css({ background: bgCss, 'background-attachment': 'fixed' });
            }
            else {
                //$("BODY").css({ 'background-color': '' });
            }

            this.$signedInBody.html(html);
        }
    });

})(window);
