using System;
using System.Web.Mvc;
using System.Web.Routing;
using ServiceStack.MiniProfiler;

namespace SocialBootstrapApi
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("Content/{*pathInfo}");
			routes.IgnoreRoute("api/{*pathInfo}"); 
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

			routes.MapRoute("CatchAll", "{*url}",
				new { controller = "Home", action = "Index" }
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			Console.WriteLine("Application_BeginRequest");
			
			if (Request.IsLocal)
				Profiler.Start();
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			Console.WriteLine("Application_EndRequest");
			Profiler.Stop();
		}
	}
}