(function (root) {
    var app = root.App;
    app.Twitter = app.BaseModel.extend({
        defaults: {
            screenName: null,
            tab: "timelines",
            timelines: [], 
            tweets: [],
            friends: [],
            followers: [],
            profile_image_url: null,
            screen_name: null,
            description: null,
            statuses_count: null,
            friends_count: null,
            followers_count: null
        },
        initialize: function() {
            _.bindAll(this, "twitterProfileChange", "twitterTab", "screenNameChanged", "tabChanged");
            this.on("change:screenName", this.screenNameChanged);
            this.on("change:tab", this.tabChanged);
        },
        isLoaded: function() {
            return !!this.get('screenName');
        },
        error: function(e) {
            $("#twitter .tab-content").html('<div class="alert alert-error">' + (e.message || e) + '</div>');
        },
        screenNameChanged: function() {
            $("BODY").toggleClass("self", this.viewingSelf());
            if (!this.get('screenName')) return;

            var self = this, profileUrl = "api/twitter/" + this.get('screenName');
            _.get(profileUrl, function (r) {
                var o = {};
                _.extend(o, r.results[0]);
                self.set(o);
                self.load(self.tab());
            }, this.error);
        },
        tabChanged: function() {
            $(".tabs ." + this.tab() + " a").tab('show');
            this.load(this.tab());
        },
        load: function(tab) {
            tab = tab || this.defaults.tab;
            var self = this, o = {}, 
                tabTweetsUrl = tab === "directmessages" ? tab : this.get('screenName') + "/" + tab;

            o[tab] = [];
            self.set(o);
            _.get("api/twitter/" + tabTweetsUrl, function (r) {
                o[tab] = r.results;
                self.set(o);
            }, this.error);
        },
        twitterProfileChange: function(screenName, tab) {
            this.set({ screenName: screenName || app.twitterScreenName(), tab: tab || this.tab() }, { silent: !app.isAuth() });
            this.navigate(this.navUrl());
        },
        viewingSelf: function() {
            return app.twitterScreenName() === this.get("screenName");
        },
        navUrl: function() {
            return (this.viewingSelf() ? "" : this.get("screenName") + "/") + this.get("tab");
        },
        tab: function() {
            return this.get('tab') || this.defaults.tab;
        },
        twitterTab: function(tab) {
            tab = tab || this.tab();
            this.set({ tab: tab });
            this.navigate(this.navUrl());
        }
    });

    app.TwitterView = app.BaseView.extend({
        initialize: function() {
            _.bindAll(this, "render");
            this.$el = $(this.el);
            this.$signedInBody = this.$el.find(".signed-in .tab-content");
            this.tweetsTemplate = _.template($("#template-tweets").html());
            this.usersTemplate = _.template($("#template-users").html());
            this.directMessagesTemplate = _.template($("#template-directmessages").html());
            this.currentUserTemplate = _.template($("#template-current-user").html());

            var changeEvents = _.map(_.keys(this.tabHooks), 
                function (k) { return "change:" + k }).join(" ");
            this.model.on(changeEvents, this.render);
        },
        tabHooks: {
            friends: function() {
                return this.usersTemplate({ users: this.model.get('friends') });
            },
            followers: function() {
                return this.usersTemplate({ users: this.model.get('followers') });
            },
            timelines: function() {
                return this.tweetsTemplate({ tweets: this.model.get('timelines') });
            },
            tweets: function() {
                return this.tweetsTemplate({ tweets: this.model.get('tweets') });
            },
            directmessages: function() {
                return this.directMessagesTemplate({ tweets: this.model.get('directmessages') });
            }
        },
        render: function() {
            var screenName = this.model.get('screenName'), html = "";
            if (screenName) {
                var tab = this.model.tab();
                html = this.tabHooks[tab].call(this);

                this.$(".current-user").html(this.currentUserTemplate(this.model.toJSON()));
                var bgImg = this.model.get('profile_background_image_url'),
                    bgColor = this.model.get('profile_background_color') || '',
                    bgRepeat = this.model.get('profile_background_tile') === "true" ? '' : 'no-repeat';
                var bgCss = bgImg ? '#' + bgColor + ' url(' + bgImg + ') ' + bgRepeat + ' top left' : '';
                $("BODY").css({ background: bgCss, 'background-attachment': 'fixed' });
            }
            this.$signedInBody.html(html);
        }
    });

})(window);
