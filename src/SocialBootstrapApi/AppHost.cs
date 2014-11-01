using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using ServiceStack;
using ServiceStack.Api.Swagger;
using ServiceStack.Auth;
using ServiceStack.Authentication.OpenId;
using ServiceStack.Caching;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using SocialBootstrapApi.Controllers;
using SocialBootstrapApi.Logic;
using SocialBootstrapApi.Models;
using SocialBootstrapApi.ServiceInterface;


/**
 * ServiceStack's Social Bootstrap API MVC web project based on 
 * Features:
 *   - MVC4 Web Application
 *   - ServiceStack web services framework hosted from /api
 *   - Twitter's Open Source HTML5 Bootstrap template
 *   - jQuery + Underscore + Backbone + Bootstrap js libraries
 *   - Bundler js/css/coffeescript compiler and minifier
 *   - Twitter + Facebook + Basic Auth + Html Form Credentials Auth modes
 *   - User Registration
 *   - Basic Backbone app showing how to structure a simple Single Page Application with Backbone
 *
 * Auto-Generated Metadata API page for all services available at: /api/metadata
 */

namespace SocialBootstrapApi
{
    //Hold App wide configuration you want to accessible by your services

    public enum Env
    {
        Local,
        Dev,
        Test,
        Prod,
    }

    //Provide extra validation for the registration process
    public class CustomRegistrationValidator : RegistrationValidator
    {
        public CustomRegistrationValidator()
        {
            RuleSet(ApplyTo.Post, () => {
                RuleFor(x => x.DisplayName).NotEmpty();
            });
        }
    }

    //The ServiceStack AppHost
    public class AppHost : AppHostBase
    {
        public AppHost() //Tell ServiceStack the name and where to find your web services  
            : base("StarterTemplate ASP.NET Host", typeof(HelloService).Assembly) { }

        public static AppConfig AppConfig;

        public override void Configure(Funq.Container container)
        {
            //Set JSON web services to return idiomatic JSON camelCase properties
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

            //Load environment config from text file if exists
            var liveSettings = "~/appsettings.txt".MapHostAbsolutePath();
            var appSettings = File.Exists(liveSettings)
                ? (IAppSettings)new TextFileSettings(liveSettings)
                : new AppSettings();
            AppConfig = new AppConfig(appSettings);
            container.Register(AppConfig);

            //Register a external dependency-free 
            container.Register<ICacheClient>(new MemoryCacheClient());
            //Configure an alt. distributed persistent cache that survives AppDomain restarts. e.g Redis
            //container.Register<IRedisClientsManager>(c => new PooledRedisClientManager("localhost:6379"));

            //Enable Authentication an Registration
            ConfigureAuth(container, appSettings);

            //Create your own custom User table
            using (var db = container.Resolve<IDbConnectionFactory>().Open())
                db.DropAndCreateTable<User>();

            //Register application services
            container.Register(new TodoRepository());
            container.Register<ITwitterGateway>(new TwitterGateway());

            //Configure Custom User Defined REST Paths for your services
            ConfigureServiceRoutes();

            //Change the default ServiceStack configuration
            //const Feature disableFeatures = Feature.Jsv | Feature.Soap;
            SetConfig(new HostConfig {
                //EnableFeatures = Feature.All.Remove(disableFeatures),
                AppendUtf8CharsetOnContentTypes = new HashSet<string> { MimeTypes.Html },
            });

            Plugins.Add(new SwaggerFeature { UseBootstrapTheme = true });
            Plugins.Add(new PostmanFeature());
            Plugins.Add(new CorsFeature());

            //Set MVC to use the same Funq IOC as ServiceStack
            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
            ServiceStackController.CatchAllController = reqCtx => container.TryResolve<HomeController>();
        }

        private void ConfigureServiceRoutes()
        {
            Routes
                //Hello World RPC example
                .Add<Hello>("/hello")
                .Add<Hello>("/hello/{Name*}")

                //Simple REST TODO example
                .Add<Todo>("/todos")
                .Add<Todo>("/todos/{Id}")

                //Custom services for this application
                .Add<Users>("/users/{UserIds}")
                .Add<UserProfile>("/profile")

                //Twitter related services
                .Add<TwitterTimelines>("/twitter/{ScreenName}/timelines")
                .Add<TwitterTweets>("/twitter/{ScreenName}/tweets")
                .Add<TwitterFriends>("/twitter/id/{UserId}/friends")
                .Add<TwitterFriends>("/twitter/{ScreenName}/friends")
                .Add<TwitterFollowers>("/twitter/id/{UserId}/followers")
                .Add<TwitterFollowers>("/twitter/{ScreenName}/followers")
                .Add<TwitterDirectMessages>("/twitter/directmessages")
                .Add<TwitterUsers>("/twitter/ids/{UserIds}") //userIds serparated by ','
                .Add<TwitterUsers>("/twitter/{ScreenNames}") //screenNames serparated by ','
            ;
        }

        private void ConfigureAuth(Funq.Container container, IAppSettings appSettings)
        {
            //Enable and register existing services you want this host to make use of.
            //Look in Web.config for examples on how to configure your oauth providers, e.g. oauth.facebook.AppId, etc.

            SetConfig(new HostConfig
            {
                DebugMode = appSettings.Get("DebugMode", false),
            });

            //Register all Authentication methods you want to enable for this web app.            
            Plugins.Add(new AuthFeature(
                () => new CustomUserSession(), //Use your own typed Custom UserSession type
                new IAuthProvider[] {
                    new CredentialsAuthProvider(),              //HTML Form post of UserName/Password credentials
                    new TwitterAuthProvider(appSettings),       //Sign-in with Twitter
                    new FacebookAuthProvider(appSettings),      //Sign-in with Facebook
                    new DigestAuthProvider(appSettings),        //Sign-in with Digest Auth
                    new BasicAuthProvider(),                    //Sign-in with Basic Auth
                    new YahooOpenIdOAuthProvider(appSettings),  //Sign-in with Yahoo OpenId
                    new OpenIdOAuthProvider(appSettings),       //Sign-in with Custom OpenId
                }));

            //Provide service for new users to register so they can login with supplied credentials.
            Plugins.Add(new RegistrationFeature());

            //override the default registration validation with your own custom implementation
            container.RegisterAs<CustomRegistrationValidator, IValidator<Register>>();

            //Create a DB Factory configured to access the UserAuth PostgreSQL DB
            var connStr = appSettings.GetString("ConnectionString");
            container.Register<IDbConnectionFactory>(
                new OrmLiteConnectionFactory(connStr, //ConnectionString in Web.Config
                    PostgreSqlDialect.Provider) {
                        ConnectionFilter = x => new ProfiledDbConnection(x, Profiler.Current)
                    });

            //Store User Data into the referenced SqlServer database
            container.Register<IUserAuthRepository>(c =>
                new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>())); //Use OrmLite DB Connection to persist the UserAuth and AuthProvider info

            var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>(); //If using and RDBMS to persist UserAuth, we must create required tables
            if (appSettings.Get("RecreateAuthTables", false))
                authRepo.DropAndReCreateTables(); //Drop and re-create all Auth and registration tables
            else
                authRepo.InitSchema();   //Create only the missing tables

            Plugins.Add(new RequestLogsFeature());
        }

        public static void Start()
        {
            new AppHost().Init();

            //To run ServiceStack and and MVC together add this to MVC RegisterRoutes(RouteCollection):
            //routes.IgnoreRoute("api/{*pathInfo}"); 
        }
    }
}
