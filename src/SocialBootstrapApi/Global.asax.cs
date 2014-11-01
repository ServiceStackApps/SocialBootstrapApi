using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ServiceStack;
using ServiceStack.MiniProfiler;

namespace SocialBootstrapApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Licensing.RegisterLicenseFromFileIfExists(@"~/appsettings.license.txt".MapHostAbsolutePath());

            if (!LicenseUtils.ActivatedLicenseFeatures().HasFlag(LicenseFeature.All))
                throw new ConfigurationErrorsException("A valid license key is required for this demo");

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            new AppHost().Init();
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