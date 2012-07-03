using System.Web.Mvc;

namespace SocialBootstrapApi.Controllers
{
    public class PublicController : ControllerBase
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}