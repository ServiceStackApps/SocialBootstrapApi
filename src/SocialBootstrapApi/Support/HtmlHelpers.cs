using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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

	public static class Html
	{
		public static MvcHtmlString Link(this HtmlHelper html, string rel, string href, object htmlAttributes = null)
		{
			if (href.IsNullOrEmpty())
				return MvcHtmlString.Empty;

			if (href.StartsWith("~/"))
				href = href.ReplaceFirst("~/", VirtualPathUtility.ToAbsolute("~"));

			var tag = new TagBuilder("link");
			tag.MergeAttribute("rel", rel);

			if (AppHost.Config.Env.In(Env.Dev, Env.Prod))
			{
				href = href.ToMinifiedUrl();
			}

			tag.MergeAttribute("href", href.ToCdn(html.ViewContext.HttpContext.Request));

			if (htmlAttributes != null)
				tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

			return tag.ToString(TagRenderMode.SelfClosing).ToMvcHtmlString();
		}

		public static MvcHtmlString Css(this HtmlHelper html, string href, string media = null)
		{
			return media != null
				? html.Link("stylesheet", href, new { media })
				: html.Link("stylesheet", href);
		}

		public static T If<T>(this HtmlHelper html, bool predicate, T whenTrue, T whenFalse)
		{
			return predicate
			       ? whenTrue
			       : whenFalse;
		}

		public static MvcHtmlString Js(this HtmlHelper html, string src)
		{
			if (src.IsNullOrEmpty())
				return MvcHtmlString.Empty;

			if (src.StartsWith("~/"))
				src = src.ReplaceFirst("~/", VirtualPathUtility.ToAbsolute("~"));

			var tag = new TagBuilder("script");
			tag.MergeAttribute("type", "text/javascript");
			if (AppHost.Config.Env.In(Env.Dev, Env.Prod) && !src.Contains("third-party") && !src.Contains("tiny_mce"))
			{
				src = src.ToMinifiedUrl();
			}
			tag.MergeAttribute("src", src.ToCdn(html.ViewContext.HttpContext.Request));
			return tag.ToString(TagRenderMode.Normal).ToMvcHtmlString();
		}

		private static readonly Regex CdnRegEx = new Regex(@"^\/content\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static string ToCdn(this string s, HttpRequestBase req, TagBuilder tag = null)
		{
			if (!AppHost.Config.EnableCdn)
				return s;

			if (s.IsNullOrEmpty())
				return s;

			if (req.IsSsl())
				return s;   // avoid browser errors

			if (!s.StartsWith("http") && !s.StartsWith("//"))
				return AppHost.Config.CdnPrefix + CdnRegEx.Replace(s, "/");

			return s;
		}

		public static MvcHtmlString Img(this HtmlHelper html, string src, string alt, string link = null, object htmlAttributes = null)
		{
			if (src.IsNullOrEmpty())
				return MvcHtmlString.Empty;

			if (src.StartsWith("~/"))
				src = src.ReplaceFirst("~/", VirtualPathUtility.ToAbsolute("~"));

			var tag = new TagBuilder("img");

			tag.MergeAttribute("src", src.ToCdn(html.ViewContext.HttpContext.Request));
			tag.MergeAttribute("alt", alt);
			if (htmlAttributes != null)
				tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

			if (!link.IsNullOrEmpty())
			{
				var a = new TagBuilder("a");
				a.MergeAttribute("href", link);
				a.InnerHtml = tag.ToString(TagRenderMode.Normal);
				return a.ToMvcHtmlString();
			}

			return tag.ToString(TagRenderMode.SelfClosing).ToMvcHtmlString();
		}

		public static MvcHtmlString Img(this HtmlHelper html, Uri url, string alt, Uri link = null, object htmlAttributes = null)
		{
			return html.Img(url.ToString(), alt, link != null ? link.ToString() : "", htmlAttributes);
		}

		public static string ToJsBool(this bool value)
		{
			return value.ToString(CultureInfo.InvariantCulture).ToLower();
		}

		static readonly ConcurrentDictionary<string, MvcHtmlString> BundleCache = new ConcurrentDictionary<string, MvcHtmlString>();
		public static MvcHtmlString RenderJsBundle(this HtmlHelper html, string bundlePath, BundleOptions options)
		{
			if (bundlePath.IsNullOrEmpty())
				return MvcHtmlString.Empty;

			var bundle = BundleCache.GetOrAdd(bundlePath, str => {
				var filePath = HttpContext.Current.Server.MapPath(bundlePath);

				var baseUrl = VirtualPathUtility.GetDirectory(bundlePath);

				if (options == BundleOptions.Combined)
					return html.Js(bundlePath.Replace(".bundle", ""));
				if (options == BundleOptions.MinifiedAndCombined)
					return html.Js(bundlePath.Replace(".js.bundle", ".min.js"));
				
				var jsFiles = File.ReadAllLines(filePath);

				var scripts = new StringBuilder();
				foreach (var file in jsFiles)
				{
					var jsFile = file.Replace(".coffee", ".js");
					var jsSrc = baseUrl.CombineWith(jsFile);
					if (options == BundleOptions.Minified && !jsSrc.EndsWith(".min.js"))
						jsSrc = jsSrc.Replace(".js", ".min.js");

					scripts.AppendLine(
						html.Js(jsSrc).ToString()
					);
				}

				return scripts.ToString().ToMvcHtmlString();
			});
			
			return bundle;
		}
	}
}