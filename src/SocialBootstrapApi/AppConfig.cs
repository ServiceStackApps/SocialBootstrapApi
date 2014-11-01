using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Configuration;

namespace SocialBootstrapApi
{
    public class AppConfig
    {
        public AppConfig(IAppSettings appSettings)
        {
            this.Env = appSettings.Get("Env", Env.Local);
            this.EnableCdn = appSettings.Get("EnableCdn", false);
            this.CdnPrefix = appSettings.Get("CdnPrefix", "");
            this.AdminUserNames = appSettings.Get("AdminUserNames", new List<string>());
        }

        public Env Env { get; set; }
        public bool EnableCdn { get; set; }
        public string CdnPrefix { get; set; }
        public List<string> AdminUserNames { get; set; }
        public BundleOptions BundleOptions
        {
            get { return Env.In(Env.Local, Env.Dev) ? BundleOptions.Normal : BundleOptions.MinifiedAndCombined; }
        }
    }
}