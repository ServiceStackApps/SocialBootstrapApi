using System.Web.Mvc;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Configuration;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using SocialBootstrapApi.Logic;
using SocialBootstrapApi.Models;
using SocialBootstrapApi.ServiceInterface;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SocialBootstrapApi.App_Start.AppHost), "Start")]


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

public static class App
{
	public static AppConfig Config = new AppConfig();

	public static IAppHost Host;
}

//Hold App wide configuration you want to accessible by your services
public class AppConfig
{
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

namespace SocialBootstrapApi.App_Start
{
	//The ServiceStack AppHost
	public class AppHost : AppHostBase
	{
		public AppHost() //Tell ServiceStack the name and where to find your web services  
			: base("StarterTemplate ASP.NET Host", typeof(HelloService).Assembly) { }

		public override void Configure(Funq.Container container)
		{
			App.Host = this;

			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

			//Register Typed Config some services might need to access
			container.Register(App.Config);

			//Register all your dependencies: 

			//Create a DB Factory configured to access the UserAuth SQL Server DB
			container.Register<IDbConnectionFactory>(
				new OrmLiteConnectionFactory(ConfigUtils.GetConnectionString("UserAuth"),
					SqlServerOrmLiteDialectProvider.Instance));

			//Register a external dependency-free 
			container.Register<ICacheClient>(new MemoryCacheClient());
			//Configure an alt. distributed peristed cache that survives AppDomain restarts. e.g Redis
			//container.Register<IRedisClientsManager>(c => new PooledRedisClientManager("localhost:6379"));

			//Create you're own custom User table
			var dbFactory = container.Resolve<IDbConnectionFactory>();
			dbFactory.Exec(dbCmd => dbCmd.CreateTable<User>(overwrite: true));

			//Register application services
			container.Register(new TodoRepository());
			container.Register<ITwitterGateway>(new TwitterGateway());

			//Define the Auth modes you support and where to store it
			ConfigureAuthAndRegistrationServices(container);

			//Configure Custom User Defined REST Paths for your services
			ConfigureServiceRoutes();

			//Change the default ServiceStack configuration
			//const Feature disableFeatures = Feature.Jsv | Feature.Soap;
			SetConfig(new EndpointHostConfig {
				//EnableFeatures = Feature.All.Remove(disableFeatures),
				DebugMode = true, //Show StackTraces in service responses during development
			});

			//Set MVC to use the same Funq IOC as ServiceStack
			ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
		}

		private void ConfigureServiceRoutes()
		{
			Routes

				//Using ServiceStack's built-in Auth and Registration services
				.Add<Auth>("/auth")
				.Add<Auth>("/auth/{provider}")
				.Add<Registration>("/register")

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
				.Add<TwitterTweets>("/twitter/{ScreenName}/tweets")
				.Add<TwitterFriends>("/twitter/id/{UserId}/friends")
				.Add<TwitterFriends>("/twitter/{ScreenName}/friends")
				.Add<TwitterFollowers>("/twitter/id/{UserId}/followers")
				.Add<TwitterFollowers>("/twitter/{ScreenName}/followers")
				.Add<TwitterUsers>("/twitter/ids/{UserIds}") //userIds serparated by ','
				.Add<TwitterUsers>("/twitter/{ScreenNames}") //screenNames serparated by ','
			;
		}

		private void ConfigureAuthAndRegistrationServices(Funq.Container container)
		{
			//Enable and register existing services you want this host to make use of.
			var appSettings = new ConfigurationResourceManager();

			//Register all Authentication methods you want to enable for this web app.
			AuthFeature.Init(this, () => new CustomUserSession(),
				new IAuthProvider[] {
					new CredentialsAuthProvider(), //HTML Form post of UserName/Password credentials
					new TwitterAuthProvider(appSettings),  //Sign-in with Twitter
					new FacebookAuthProvider(appSettings), //Sign-in with Facebook
					new BasicAuthProvider(), //Sign-in with Basic Auth
				});

			//Provide service for new users to register so they can login with supplied credentials.
			RegistrationFeature.Init(this);

			//override the default registration validation
			container.RegisterAs<CustomRegistrationValidator, IValidator<Registration>>();

			//Store User Data into the referenced SqlServer database
			container.Register<IUserAuthRepository>(c =>
				new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
			authRepo.DropAndReCreateTables(); //Drop and re-create all Auth and registration tables
			//authRepo.CreateMissingTables(); //Create only the missing tables
		}

		public static void Start()
		{
			new AppHost().Init();

			//To run ServiceStack and and MVC together add this to MVC RegisterRoutes(RouteCollection):
			//routes.IgnoreRoute("api/{*pathInfo}"); 
		}
	}
}
