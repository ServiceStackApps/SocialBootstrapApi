/// <reference path="base.js" />
/// <reference path="login.js" />
/// <reference path="register.js" />
/// <reference path="userprofile.js" />
(function (root)
{
	var app = root.App;
	var login = new app.Login();

	_.extend(app, {
		UnAuthorized: 401,
		login: login,
		userProfile: new app.UserProfile({ loginModel: login }),
		initialize: function ()
		{
			_.bindAll(this, "error");
		},
		error: function (xhr, err, statusText)
		{
			console.log("App Error: ", arguments);
			this.trigger("error", arguments);
			if (xhr.status == this.UnAuthorized)
			{
				//verify user is no longer authenticated
				$.getJSON("/userinfo", function (r) { }, function (xhr)
				{
					if (xhr.status == this.UnAuthorized)
						location.href = location.href;
				});
			}
		}
	});
	_.extend(app, Backbone.Events);

	console.log(app.login, app.userProfile);

	app.views = {
		login: new app.LoginView({
			el: "#login",
			model: app.login
		}),
		register: new app.RegisterView({
			el: "#register",
			model: app.login
		}),
		userProfile: new app.UserProfileView({
			el: "#user-profile",
			model: app.userProfile
		})
	};

	app.initialize();

})(window);
