(function (root) 
{
	var app = root.App;

	_.extend(app, {
	    baseUrl: window.BASE_URL,
		UnAuthorized: 401,
		initialize: function ()
		{
			this.handleClicks();
		},
		handleClicks: function ()
		{
			$(document.body).click(function (e)
			{
			    console.log("handleClicks", e);
				var dataCmd = $(e.srcElement).data('cmd');
				if (!dataCmd) return;

				var cmd = dataCmd.split(':'),
					evt = cmd[0],
					args = cmd.length > 1 ? cmd[1].split(',') : [];

				app.sendCmd(evt, args);
			});
		},
		navigate: function(path) {
		    this.routes.navigate(path);
		},
		route: function (evt) {
		    var args = _.rest(arguments);
		    console.log("route: " + evt, args);
		    this.sendCmd(evt, args);
		},
		sendCmd: function (evt, args)
		{
		    if (_.isFunction(this.routes[evt])) 
		        this.routes[evt].apply(this.routes, args);
		    
			_.each(this.models, function (el) {
				if (_.isFunction(el[evt])) el[evt].apply(el, args);
			});
			_.each(this.views, function (el) {
				if (_.isFunction(el[evt])) el[evt].apply(el, args);
			});
		},
		error: function (xhr, err, statusText)
		{
			console.log("App Error: ", arguments);
			this.trigger("error", arguments);
			if (xhr.status == this.UnAuthorized)
			{
				//verify user is no longer authenticated
				$.getJSON("api/userinfo", function (r) { }, function (xhr) {
					if (xhr.status == this.UnAuthorized)
						location.href = location.href;
				});
			}
		}
	});
	_.extend(app, Backbone.Events);
	_.bindAll(app, "error", "trigger", "navigate", "sendCmd");  
    
	var login = new app.Login();
	var userProfile = new app.UserProfile({ login: login });
    var twitter = new app.Twitter({ profile: userProfile });
    
    app.Routes = Backbone.Router.extend({
        routes: {
            "tweets": "tweets",
            "friends": "friends",
            "followers": "followers",
            "userTweets": ":user/tweets",
            "userFriends": ":user/friends",
            "userFollowers": ":user/followers"
        },
        initialize: function (opt) {
            this.app = opt.app;
        },
        tweets: function () {
            this.app.route("twitterProfileChange", login.get('screenName'), "tweets");
        },
        friends: function () {
            this.app.route("twitterProfileChange", login.get('screenName'), "friends");
        },
        followers: function () {
            this.app.route("twitterProfileChange", login.get('screenName'), "followers");
        },
        userTweets: function (user) {
            this.app.route("twitterProfileChange", user, "tweets");
        },
        userFriends: function (user) {
            this.app.route("twitterProfileChange", user, "friends");
        },
        userFollowers: function (user) {
            this.app.route("twitterProfileChange", user, "followers");
        }
    });
    app.routes = new app.Routes({app:app});

    console.log(login.attributes, userProfile.attributes, twitter.attributes);

	app.models = {
		login: login,
		userProfile: userProfile,
		twitter: twitter
	};
    _.each(app.models, function(model) {
        model.sendCmd = app.sendCmd;
        model.navigate = app.navigate;
    })

	app.views = {
		login: new app.LoginView({
			el: "#login",
			model: login
		}),
		register: new app.RegisterView({
			el: "#register",
			model: login
		}),
		userProfile: new app.UserProfileView({
			el: "#user-profile",
			model: userProfile
		}),
        twitter: new app.TwitterView({
            el: "#twitter",
            model: twitter
		})
	};

	app.initialize();
    $(".tabs").tabs();


    Backbone.history.start({ pushState: true }); //{ pushState: true }
    $(function () {
    });

})(window);
