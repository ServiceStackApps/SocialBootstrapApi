using System.Web.Mvc;
using ServiceStack.ServiceInterface;

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