using System.Collections.Generic;
using System.Web.Mvc;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common.Web;
using ServiceStack.Configuration;
using ServiceStack.FluentValidation;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using SocialBootstrapApi.Controllers;
using SocialBootstrapApi.Logic;
using SocialBootstrapApi.Models;
using SocialBootstrapApi.ServiceInterface;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SocialBootstrapApi.AppHost), "Start")]


/**
 * ServiceStack's Social Bootstrap API MVC web project based on 
 * Features:
 *   - MVC3 Web Application
 *   - ServiceStack web services framework hosted from /api
 *   - Twitter's Open Source HTML5 Bootstrap template
 *   - jQuery + Underscore + Backbone + Bootstrap js libraries
 *   - Cassette.Web js/css/coffeescript compiler and minifier
 *   - Twitter + Facebook + Basic Auth + Html Form Credentials Auth modes
 *   - User Registration
 *   - Basic Backbone app showing how to structure a simple Single Page Application with Backbone
 *
 * Auto-Generated Metadata API page for all services available at: /api/metadata
 */

namespace SocialBootstrapApi
{
	//Hold App wide configuration you want to accessible by your services
	public class AppConfig
	{
		public AppConfig(IResourceManager appSettings)
		{
			this.Env = appSettings.Get("Env", Env.Local);
			this.EnableCdn = appSettings.Get("EnableCdn", false);
			this.CdnPrefix = appSettings.Get("CdnPrefix", "");
		}

		public Env Env { get; set; }
		public bool EnableCdn { get; set; }
		public string CdnPrefix { get; set; }
		public BundleOptions BundleOptions
		{
			get { return Env.In(Env.Local, Env.Dev) ? BundleOptions.Normal : BundleOptions.MinifiedAndCombined; }
		}
	}

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

		public static AppConfig Config;

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

			//Register Typed Config some services might need to access
			var appSettings = new AppSettings();
			Config = new AppConfig(appSettings);
			container.Register(Config);

			//Register all your dependencies: 

			//Register a external dependency-free 
			container.Register<ICacheClient>(new MemoryCacheClient());
			//Configure an alt. distributed peristed cache that survives AppDomain restarts. e.g Redis
			//container.Register<IRedisClientsManager>(c => new PooledRedisClientManager("localhost:6379"));

			//Enable Authentication an Registration
			ConfigureAuth(container);

			//Create you're own custom User table
			var dbFactory = container.Resolve<IDbConnectionFactory>();
			dbFactory.Exec(dbCmd => dbCmd.CreateTable<User>(overwrite: true));

			//Register application services
			container.Register(new TodoRepository());
			container.Register<ITwitterGateway>(new TwitterGateway());

			//Configure Custom User Defined REST Paths for your services
			ConfigureServiceRoutes();

			//Change the default ServiceStack configuration
			//const Feature disableFeatures = Feature.Jsv | Feature.Soap;
			SetConfig(new EndpointHostConfig {
				//EnableFeatures = Feature.All.Remove(disableFeatures),
				AppendUtf8CharsetOnContentTypes = new HashSet<string> { ContentType.Html },
				DebugMode = true, //Show StackTraces in service responses during development
			});

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

		private void ConfigureAuth(Funq.Container container)
		{
			//Enable and register existing services you want this host to make use of.
			//Look in Web.config for examples on how to configure your oauth proviers, e.g. oauth.facebook.AppId, etc.
			var appSettings = new AppSettings();

			//Register all Authentication methods you want to enable for this web app.            
			Plugins.Add(new AuthFeature(
				() => new CustomUserSession(), //Use your own typed Custom UserSession type
				new IAuthProvider[] {
                    new CredentialsAuthProvider(),         //HTML Form post of UserName/Password credentials
                    new TwitterAuthProvider(appSettings),  //Sign-in with Twitter
                    new FacebookAuthProvider(appSettings), //Sign-in with Facebook
                    new BasicAuthProvider(),               //Sign-in with Basic Auth
                }));

			//Provide service for new users to register so they can login with supplied credentials.
			Plugins.Add(new RegistrationFeature());

			//override the default registration validation with your own custom implementation
			container.RegisterAs<CustomRegistrationValidator, IValidator<Registration>>();

			//Create a DB Factory configured to access the UserAuth SQL Server DB
			container.Register<IDbConnectionFactory>(
				new OrmLiteConnectionFactory(ConfigUtils.GetConnectionString("UserAuth"), //ConnectionString in Web.Config
					SqlServerOrmLiteDialectProvider.Instance) {
						ConnectionFilter = x => new ProfiledDbConnection(x, Profiler.Current)
					});

			//Store User Data into the referenced SqlServer database
			container.Register<IUserAuthRepository>(c =>
				new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>())); //Use OrmLite DB Connection to persist the UserAuth and AuthProvider info

			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>(); //If using and RDBMS to persist UserAuth, we must create required tables
			if (appSettings.Get("RecreateAuthTables", false))
				authRepo.DropAndReCreateTables(); //Drop and re-create all Auth and registration tables
			else
				authRepo.CreateMissingTables();   //Create only the missing tables
		}

		public static void Start()
		{
			new AppHost().Init();

			//To run ServiceStack and and MVC together add this to MVC RegisterRoutes(RouteCollection):
			//routes.IgnoreRoute("api/{*pathInfo}"); 
		}
	}
}
