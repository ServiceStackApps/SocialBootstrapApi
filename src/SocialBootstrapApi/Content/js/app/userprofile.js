/// <reference path="base.js" />
/// <reference path="login.js" />
(function (root)
{
	var app = root.App;

	app.UserProfile = app.BaseModel.extend({
		urlRoot: "/api/profile",
		defaults: {
			Id: 0,
			UserName: null,
			DisplayName: null,
			loginModel: null,
			ProfileImageUrl64: null
		},
		initialize: function (opt)
		{
			_.bindAll(this, "authChange");
			opt.loginModel.bind("change:isAuthenticated", this.authChange);
		},
		authChange: function ()
		{
			console.log("AuthChange: ");
			this.fetch();
		}
	});

	app.UserProfileView = app.BaseView.extend(
		{
			initialize: function ()
			{
				_.bindAll(this, "render");
				this.model.bind("change", this.render);
				this.$el = $(this.el);
				this.template = _.template($("#template-userprofile").html());
			},
			render: function ()
			{
				this.$el.hide();
				var attrs = this.model.attributes;
				attrs.TwitterUserId = attrs.TwitterUserId || null;
				attrs.FacebookUserId = attrs.FacebookUserId || null;
				console.log(attrs);				
				var html = this.template(attrs);
				this.$el.html(html);
				this.$el.fadeIn('fast');

				if (attrs.FacebookUserId)
					$("#facebook-signin").hide();
				else
					$("#facebook-signin").show();

				if (attrs.TwitterUserId)
					$("#twitter-signin").hide();
				else
					$("#twitter-signin").show();

			}
		});

})(window);
