(function (root)
{
	var app = root.App;
	app.UserProfile = app.BaseModel.extend({
		url: "api/profile",
		defaults: {
			id: null,
			email: null,
			userName: null,
			displayName: null,
			twitterUserId: null,
			twitterScreenName: null,
			twitterName: null,
			facebookName: null,
			facebookFirstName: null,
			facebookLastName: null,
			facebookUserId: null,
			facebookUserName: null,
			facebookEmail: null,
			gravatarImageUrl64: null,
			showProfile: null
		},
		initialize: function(opt) {
		},
		hasTwitterAuth: function() {
		    return !!this.get('twitterUserId');
		}
	});

	app.UserProfileView = app.BaseView.extend({
		initialize: function() {
			_.bindAll(this, "render");
			this.model.bind("change", this.render);
			this.$el = $(this.el);
			this.template = _.template($("#template-userprofile").html());
		},
		render: function() {
			this.$el.hide();
			var attrs = this.model.attributes;
			attrs.twitterUserId = attrs.twitterUserId || null;
			attrs.facebookUserId = attrs.facebookUserId || null;

			console.log(attrs);
			$("BODY").toggleClass("authenticated-twitter", !!attrs.twitterUserId);
			$("BODY").toggleClass("authenticated-facebook", !!attrs.facebookUserId);

			var showProfile = attrs.email || attrs.twitterUserId || attrs.facebookUserId;
			if (showProfile) {
				var html = this.template(attrs);
				this.$el.html(html);
				this.$el.fadeIn('fast');
			} else {
				this.$el.html("");
				this.$el.hide();
			}
        }
	});

})(window);
