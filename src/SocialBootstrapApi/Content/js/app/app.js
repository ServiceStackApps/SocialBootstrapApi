/// <reference path="base.js" />
/// <reference path="login.js" />
/// <reference path="register.js" />
/// <reference path="userprofile.js" />
(function (root)
{
	var app = root.App;

	_.extend(app, {
		UnAuthorized: 401,
		initialize: function ()
		{
			_.bindAll(this, "error", "trigger");
			this.handleClicks();
		},
		handleClicks: function ()
		{
			$(document.body).click(function (e)
			{
				var dataCmd = $(e.srcElement).data('cmd');
				if (!dataCmd) return;

				var cmd = dataCmd.split(':'),
					evt = cmd[0],
					args = cmd.length > 1 ? cmd[1].split(',') : [];

				app.sendCmd(evt, args);
			});
		},
		sendCmd: function (evt, args)
		{
			_.each(this.models, function (el) {
				if (el[evt]) el[evt].apply(el, args);
			});
			_.each(this.views, function (el) {
				if (el[evt]) el[evt].apply(el, args);
			});
		},
		error: function (xhr, err, statusText)
		{
			console.log("App Error: ", arguments);
			this.trigger("error", arguments);
			if (xhr.status == this.UnAuthorized)
			{
				//verify user is no longer authenticated
				$.getJSON("api/userinfo", function (r) { }, function (xhr)
				{
					if (xhr.status == this.UnAuthorized)
						location.href = location.href;
				});
			}
		}
	});
	_.extend(app, Backbone.Events);

	var login = new app.Login();
	var userProfile = new app.UserProfile({ login: login });
	console.log(login.attributes, userProfile.attributes);

	app.models = {
		login: login,
		userProfile: userProfile
	};

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
		})
	};

	app.initialize();

})(window);
