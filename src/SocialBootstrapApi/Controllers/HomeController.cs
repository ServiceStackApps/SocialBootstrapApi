using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SocialBootstrapApi.App_Start;
using SocialBootstrapApi.Models;
using ControllerBase = SocialBootstrapApi.App_Start.ControllerBase;

namespace SocialBootstrapApi.Controllers
{
	public class HomeController : ControllerBase
	{
		public AppConfig Config { get; set; }

		public ActionResult Index()
		{
			ViewBag.Message = "MVC + ServiceStack PowerPack!";
			ViewBag.UserSession = base.AuthUserSession;
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
