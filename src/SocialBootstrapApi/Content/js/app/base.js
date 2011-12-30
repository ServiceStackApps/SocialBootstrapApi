/// <reference path="../jquery-1.7.js" />
/// <reference path="../underscore.js" />
/// <reference path="../backbone.js" />
(function (root)
{
	var app = root.App = root.App || {};

	_.mixin({
		formData: function (form)
		{
			var ret = {};
			$(form).find("INPUT,TEXTAREA").each(function ()
			{
				if (this.type == "button" || this.type == "submit") return;
				ret[this.name] = $(this).val();
			});
			return ret;
		},
		xhrMessage: function (xhr)
		{
			try
			{
				var respObj = JSON.parse(xhr.responseText);
				if (!respObj.responseStatus) return null;
				return respObj.responseStatus.message;
			}
			catch (e)
			{
				return null;
			}
		}
	});


	app.BaseModel = Backbone.Model.extend({
		parse: function (resp, xhr)
		{
			if (!resp) return resp;
			return resp.result || resp.results || resp;
		},
		_super: function (funcName)
		{
			return this.constructor.__super__[funcName].apply(this, _.rest(arguments));
		}
	});

	app.BaseView = Backbone.View.extend({
		get: function (url, data, success, error)
		{
			$.getJSON(url, data, success, error || App.error);
		},
		post: function (url, data, success, error)
		{
			var $this = this;
			$this.loading();
			$.ajax({
				type: 'POST',
				url: url,
				data: data,
				success: function ()
				{
					console.log(arguments);
					$this.finishedLoading();
					if (success) success.apply(null, arguments);
				},
				error: function ()
				{
					console.log(arguments);
					$this.finishedLoading();
					(error || App.error).apply(null, arguments);
				},
				dataType: "json"
			});
		},
		loading: function ()
		{
			$(this.el).css({ opacity: 0.5 });
		},
		finishedLoading: function ()
		{
			$(this.el).css({ opacity: 1 });
		}
	});

})(window);
