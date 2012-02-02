using System.Web.Mvc;

namespace SocialBootstrapApi.Controllers
{
	public partial class HomeController : ControllerBase
	{
		public AppConfig Config { get; set; }

		public virtual ActionResult Index()
		{
			ViewBag.Message = "MVC + ServiceStack PowerPack!";
			ViewBag.UserSession = base.UserSession;
			ViewBag.Config = Config;

			return View();
		}

		public virtual ActionResult About()
		{
			return View();
		}

		public virtual ActionResult Features()
		{
			return View();
		}
	}
}
