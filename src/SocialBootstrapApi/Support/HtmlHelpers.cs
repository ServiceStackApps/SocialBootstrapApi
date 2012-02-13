using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Common;

namespace SocialBootstrapApi
{
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