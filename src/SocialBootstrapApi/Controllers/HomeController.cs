using System.Web.Mvc;

namespace SocialBootstrapApi.Controllers
{
	public class HomeController : ControllerBase
	{
		public AppConfig Config { get; set; }

		public ActionResult Index()
		{
			ViewBag.Message = "MVC + ServiceStack PowerPack!";
			ViewBag.UserSession = base.UserSession;
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
