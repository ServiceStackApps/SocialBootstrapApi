/// <reference path="base.js" />
(function (root)
{
	var app = root.App;

	app.RegisterView = app.BaseView.extend(
		{
			className: "view-register",			
			initialize: function ()
			{
				_.bindAll(this, "render", "register", "registerSuccess", "registerError",
					"login", "loginSuccess", "loginError");

				this.model.bind("change", this.render);

				this.$("[name=DisplayName]").val("Demis");
				this.$("[name=Email]").val("demis.bellot@gmail.com");
				this.$("[name=Password]").val("test");

				this.$el = $(this.el);
				this.$errorMsg = this.$el.find("form b");
				this.$signup = $(this.el).find("#signup");
				this.$registerLogin = $(this.el).find("#register-login");
				this.$active = this.$signup;

				this.$signup.find("form").submit(this.register);
				this.$registerLogin.find("form").submit(this.login);
			},
			register: function (e)
			{
				if (e) e.preventDefault();

				var form = this.$signup.find("form");
				this.post(form.attr("action"), _(form).formData(), this.registerSuccess, this.registerError);

				this.$registerLogin.find("INPUT[name=UserName]").val(
					form.find("INPUT[name=Email]").val());
			},
			registerSuccess: function (r)
			{
				this.$el.removeClass("error");
				this.model.set({ hasRegistered: true, userId: r.UserId });
			},
			registerError: function (xhr)
			{
				this.$el.addClass("error");
				this.$errorMsg.html(_(xhr).xhrMessage());
			},
			login: function (e)
			{
				if (e) e.preventDefault();

				var form = this.$registerLogin.find("form");
				this.post(form.attr("action"), _(form).formData(), this.loginSuccess, this.loginError);
			},
			loginSuccess: function (r)
			{
				this.$el.removeClass("error");
				this.model.set({ isAuthenticated: true });
			},
			loginError: function ()
			{
				this.$el.addClass("error");
			},
			render: function ()
			{
				var $this = this;
				this.$errorMsg.html("");
				if (this.model.get("isAuthenticated"))
				{
					this.$active.fadeOut("fast");
				}
				else if (this.model.get("hasRegistered"))
				{
					if (this.$active == this.$registerLogin) return;

					this.$active.fadeOut("fast", function ()
					{
						$this.$registerLogin.fadeIn("fast");
						$this.$active = $this.$registerLogin;
					});
				}
				else
				{
					if (this.$active == this.$signup) return;

					this.$active.fadeOut("fast", function ()
					{
						$this.$signup.fadeIn("fast");
						$this.$active = $this.$signup;
					});
				}
			}
		}
	);

})(window);
