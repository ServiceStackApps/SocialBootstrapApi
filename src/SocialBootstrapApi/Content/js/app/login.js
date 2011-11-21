/// <reference path="base.js" />
(function (root)
{
	var app = root.App;
	
	app.Login = app.BaseModel.extend({
		urlRoot: "/api/auth/credentials",
		defaults: {
			isAuthenticated: false,
			hasRegistered: false,
			sessionId: null,
			userId: null
		}
	});

	app.LoginView = app.BaseView.extend(
		{
			className: "view-login",

			initialize: function ()
			{
				_.bindAll(this, "login", "render", "loginSuccess", "loginError");
				$(this.el).find("form").submit(this.login);
			},
			login: function (e)
			{
				if (e) e.preventDefault();

				var form = $(this.el).find("form");
				this.post(form.attr("action"), _(form).formData(), this.loginSuccess, this.loginError);
			},
			loginSuccess: function (r)
			{
				$(this.el).removeClass("error");
				this.model.set({ isAuthenticated: true });
			},
			loginError: function ()
			{
				$(this.el).addClass("error");
			},

			render: function ()
			{
			}
		}
	);

})(window);
