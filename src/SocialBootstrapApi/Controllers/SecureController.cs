using System.Web.Mvc;
using ServiceStack;

namespace SocialBootstrapApi.Controllers
{
    [Authenticate]
    public class SecureController : ControllerBase
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}