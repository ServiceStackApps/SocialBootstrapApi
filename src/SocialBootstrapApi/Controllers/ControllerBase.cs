using ServiceStack.Mvc;
using ServiceStack.Mvc.MiniProfiler;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.Controllers
{
	[ProfilingActionFilter]
	public class ControllerBase : ControllerBase<CustomUserSession>
	{
		 
	}
}