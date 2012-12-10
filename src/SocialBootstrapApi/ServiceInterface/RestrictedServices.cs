using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace SocialBootstrapApi.ServiceInterface
{
    [Restrict(EndpointAttributes.Json)]
    public class JsonOnly { }

    [Restrict(EndpointAttributes.Xml)]
    public class XmlOnly { }

    [Restrict(VisibilityTo = EndpointAttributes.Json)]
    public class VisibleToJsonOnly { }

    [Restrict(VisibilityTo = EndpointAttributes.Xml)]
    public class VisibleToXmlOnly { }

    [Restrict(EndpointAttributes.Localhost)]
    public class LocalhostOnly { }

    [Restrict(EndpointAttributes.InternalNetworkAccess)]
    public class InternalOnly { }

    [Restrict(EndpointAttributes.External)]
    public class ExternalOnly { }

    public class GetOnly { }
    public class PostOnly { }

    public class Response
    {
        public string Result { get; set; }
    }

    public class RestrictedServices : Service
    {
        public Response Any(JsonOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(XmlOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(VisibleToJsonOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(VisibleToXmlOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(LocalhostOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(InternalOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(ExternalOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(GetOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Any(PostOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

    }
}