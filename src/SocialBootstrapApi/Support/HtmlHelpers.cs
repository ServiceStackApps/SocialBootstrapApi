using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Common;

namespace SocialBootstrapApi
{
	public enum BundleOptions
	{
		Normal,
		Minified,
		Combined,
		MinifiedAndCombined
	}

	public static class HelperExtensions
	{
		public static bool In(this Env env, params Env[] inAnyEnvs)
		{
			return inAnyEnvs.Any(x => x == env);
		}

		public static string ToMinifiedUrl(this string s, bool doMin = true)
		{
			if (!doMin || s.IsNullOrEmpty() || s.Contains(".min.")) return s;
			return s.Replace(".js", ".min.js").Replace(".css", ".min.css");
		}

		public static MvcHtmlString ToMvcHtmlString(this string s)
		{
			return MvcHtmlString.Create(s);
		}

		public static MvcHtmlString ToMvcHtmlString(this TagBuilder t)
		{
			return t.ToString().ToMvcHtmlString();
		}

		public static MvcHtmlString ToMvcHtmlString(this TagBuilder t, TagRenderMode mode)
		{
			return t.ToString(mode).ToMvcHtmlString();
		}

		public static bool IsSsl(this HttpRequest request)
		{
			return request.IsSecureConnection;
		}

		public static bool IsSsl(this HttpRequestBase request)
		{
			return request.IsSecureConnection;
		}
	}
}