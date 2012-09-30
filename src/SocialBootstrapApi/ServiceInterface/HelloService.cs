using ServiceStack.ServiceInterface;

namespace SocialBootstrapApi.ServiceInterface
{
	//Request DTO
	public class Hello
	{
		public string Name { get; set; }
	}

	//Response DTO
	public class HelloResponse
	{
		public string Result { get; set; }
	}

	//Can be called via any endpoint or format, see: http://servicestack.net/ServiceStack.Hello/
	public class HelloService : Service
	{
		//Get's called by all HTTP Verbs (GET,POST,PUT,DELETE,etc) and endpoints JSON,XMl,JSV,etc
	    public object Any(Hello request)
		{
			return new HelloResponse { Result = "Hello, Olle är en ÖL ål " + request.Name };
		}
	}
}