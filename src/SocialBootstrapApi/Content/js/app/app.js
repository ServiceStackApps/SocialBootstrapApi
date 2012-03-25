(function (root) 
{
    if (!$("#twitter")[0]) return;
    $("#page-body").addClass("app-loading");

	var app = root.App;

	_.extend(app, {
	    baseUrl: window.BASE_URL,
		UnAuthorized: 401,
	    hasLoaded: false,
		start: function () {
		    Backbone.history.start({ pushState: true });
		    this.handleClicks();
		},
		handleClicks: function() {
			$(document.body).click(function (e) {
			    console.log("handleClicks", e);
				var dataCmd = $(e.srcElement).data('cmd');
				if (!dataCmd) return;

				var cmd = dataCmd.split(':'),
					evt = cmd[0],
					args = cmd.length > 1 ? cmd[1].split(',') : [];

				app.sendCmd(evt, args);
			});
		},
        finishedLoading: function() {
	        if (!this.hasLoaded) {
	            this.hasLoaded = true;
	            $(".app-loading").removeClass("app-loading");
	            console.log("app finishedLoading...");
	        }
	    },
		navigate: function(path) {
		    this.routes.navigate(path);
		},
		route: function(evt) {
		    var args = _.rest(arguments);
		    console.log("route: " + evt, args);
		    this.sendCmd(evt, args);
		    this.finishedLoading();
		},
		sendCmd: function(evt, args) {
		    if (_.isFunction(this.routes[evt])) 
		        this.routes[evt].apply(this.routes, args);
		    
			_.each(this.models, function (el) {
				if (_.isFunction(el[evt])) el[evt].apply(el, args);
			});
			_.each(this.views, function (el) {
				if (_.isFunction(el[evt])) el[evt].apply(el, args);
			});
		},
		error: function(xhr, err, statusText) {
			console.log("App Error: ", arguments);
			this.trigger("error", (err || xhr));
			if (xhr.status == this.UnAuthorized) {
				//verify user is no longer authenticated
				$.getJSON("api/userinfo", function (r) { }, function (xhr) {
					if (xhr.status == this.UnAuthorized)
						location.href = location.href;
				});
			}
		}
	});
	_.extend(app, Backbone.Events);
	_.bindAll(app, "error", "navigate", "sendCmd");  
    
	var login = new app.Login();
	var userProfile = new app.UserProfile({ login: login });
    var twitter = new app.Twitter();

    app.isAuth = function() {
        return login.get("isAuthenticated");
    };
    app.twitterScreenName = function() {
        return userProfile.get("twitterScreenName");
    };

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

    $(".tabs a:first").tab('show');
    $(".dropdown-toggle").dropdown();

})(window);
