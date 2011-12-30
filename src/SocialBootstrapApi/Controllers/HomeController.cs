using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SocialBootstrapApi.App_Start;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.Controllers
{
	public class ControllerBase : Controller
	{
		protected string SessionKey
		{
			get
			{
				var cookie = base.Request.Cookies.Get(SessionFeature.PermanentSessionId);
				return cookie == null ? null : AuthService.GetSessionKey(cookie.Value);
			}
		}

		private CustomUserSession userSession;
		protected CustomUserSession UserSession
		{
			get
			{
				if (userSession != null) return userSession;
				if (SessionKey != null)
				{
					using (var cacheClient = App.Host.GetCacheClient())
						userSession = cacheClient.Get<CustomUserSession>(SessionKey);
				}
				var unAuthorizedSession = new CustomUserSession();
				return userSession ?? (userSession = unAuthorizedSession);
			}
		}
	}

	public class HomeController : ControllerBase
	{
		public AppConfig Config { get; set; }

		public ActionResult Index()
		{
			ViewBag.Message = "MVC + ServiceStack PowerPack!";
			ViewBag.UserSession = UserSession;
			ViewBag.Config = Config;

			return View();
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Features()
		{
			return View();
		}
	}
}
