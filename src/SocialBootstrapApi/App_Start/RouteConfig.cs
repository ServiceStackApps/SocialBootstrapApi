using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ServiceStack;
using ServiceStack.Mvc;
using ServiceStack.Web;

namespace SocialBootstrapApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("api/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Forwarding", "auth/{*pathinfo}", new { controller = "Forwarding", action = "Index" });
           
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("CatchAll", "{*url}",
                new { controller = "Home", action = "Index" }
            );
        }
    }


    /// <summary>
    /// Re-use existing AuthWebTests OAuth app config which have callbacks registered at:
    /// http://localhost:11001/auth/{provider} instead of the expected path for MVC + SS: 
    /// http://localhost:11001/api/auth/{provider}
    /// </summary>
    public class ForwardingController : ServiceStackController
    {
        public ActionResult Index()
        {
            var response = ForwardRequestToServiceStack();
            if (ServiceStackResponse.IsClosed) return new EmptyResult();

            if (response is IHttpResult httpResult)
            {
                if (httpResult.Headers.TryGetValue(HttpHeaders.Location, out var redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
            }

            return Redirect("/");
        }
    }
}