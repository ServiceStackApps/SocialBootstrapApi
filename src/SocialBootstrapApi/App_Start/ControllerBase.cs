using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ServiceStack.CacheAccess;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.App_Start
{
	public abstract class ControllerBase : Controller
	{
		public ICacheClient Cache { get; set; }
		public ISessionFactory SessionFactory { get; set; }

		private ISession session;
		public new ISession Session
		{
			get
			{
				return session ?? (session = SessionFactory.GetOrCreateSession());
			}
		}

		protected string SessionKey
		{
			get
			{
				var sessionId = SessionFeature.GetSessionId();
				return sessionId == null ? null : SessionFeature.GetSessionKey(sessionId);
			}
		}

		private CustomUserSession userSession;
		protected CustomUserSession UserSession
		{
			get
			{
				if (userSession != null) return userSession;
				if (SessionKey != null)
					userSession = this.Cache.Get<CustomUserSession>(SessionKey);
				else
					SessionFeature.CreateSessionIds();

				var unAuthorizedSession = new CustomUserSession();
				return userSession ?? (userSession = unAuthorizedSession);
			}
		}
		
		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new ServiceStackJsonResult {
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding
			};
		}
	}

	public class ServiceStackJsonResult : JsonResult
	{
		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

			if (ContentEncoding != null)
			{
				response.ContentEncoding = ContentEncoding;
			}

			if (Data != null)
			{
				response.Write(JsonSerializer.SerializeToString(Data));
			}
		}
	}
}