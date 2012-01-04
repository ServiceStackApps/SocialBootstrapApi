/// <reference path="../jquery-1.7.js" />
/// <reference path="../underscore.js" />
/// <reference path="../backbone.js" />
/// <reference path="ss-validation.js" />
(function (root)
{
    $.ss.validation.overrideMessages = true;

	var app = root.App = root.App || {};
    var emptyFn = function() {};
    
	_.mixin({
		formData: function (form)
		{
			var ret = {};
			$(form).find("INPUT,TEXTAREA").each(function() {
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
		},
        get: function (url, data, success, error) {
            if (_.isFunction(data)) {
                success = data;
                error = success;
                data = undefined;
            }
            return _.ajax({
                type: 'GET',
                url: url,
                data: data,
                success: success,
                error: error
            });
        },
		post: function (opt) {
		    return _.ajax(opt);
		},
        ajax: function (opt)
		{
            var o = _.defaults(opt, {
               type: 'POST',
               loading: function() {
                   $(document.body).add(opt.form).addClass("loading");
               },
               finishedLoading: function() {
                   $(document.body).add(opt.form).removeClass("loading");
               },
               dataType: "json"
            });
			o.loading();
			$.ajax({
				type: o.type,
				url: o.url,
				data: o.data,
				success: function()
				{
					//console.log(arguments);
					o.finishedLoading();
				    $(o.form).clearErrors();
					if (o.success) o.success.apply(null, arguments);
				},
				error: function(xhr,err,status)
				{
					//console.log(arguments);
					o.finishedLoading();
				    try {
				        if (o.form) {
				            var r = JSON.parse(xhr.responseText);
				            $(o.form).applyErrors(r && r.responseStatus);
				        }
				    } catch(e){}
					(o.error || (app.error || emptyFn)).apply(null, arguments);
				},
				dataType: o.dataType
			});
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
