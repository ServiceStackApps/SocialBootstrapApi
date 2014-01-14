using ServiceStack;

namespace SocialBootstrapApi.ServiceInterface
{
    [Restrict(RequestAttributes.Json)]
    public class JsonOnly { }

    [Restrict(RequestAttributes.Xml)]
    public class XmlOnly { }

    [Restrict(VisibilityTo = RequestAttributes.Json)]
    public class VisibleToJsonOnly { }

    [Restrict(VisibilityTo = RequestAttributes.Xml)]
    public class VisibleToXmlOnly { }

    [Restrict(RequestAttributes.Localhost)]
    public class LocalhostOnly { }

    [Restrict(RequestAttributes.InternalNetworkAccess)]
    public class InternalOnly { }

    [Restrict(RequestAttributes.External)]
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

        public Response Get(GetOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

        public Response Post(PostOnly request)
        {
            return new Response { Result = "Haz Sekritz!" };
        }

    }
}