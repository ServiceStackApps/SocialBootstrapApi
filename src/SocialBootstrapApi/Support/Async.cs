using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Logging;

namespace SocialBootstrapApi.Support
{
	public static class Async
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Async));

		public static Task Success<TResult>(this Task<TResult> task, Action<Task<TResult>> successor)
		{
			return task.ContinueWith(_ => {
				if (task.IsCanceled || task.IsFaulted)
				{
					return task;
				}
				return Task.Factory.StartNew(() => successor(task));
			}).Unwrap();
		}

		public static Task<TResult> Success<TResult>(this Task task, Func<Task, TResult> successor)
		{
			return task.ContinueWith(_ => {
				if (task.IsFaulted)
				{
					return FromError<TResult>(task.Exception);
				}
				if (task.IsCanceled)
				{
					return Cancelled<TResult>();
				}
				return Task.Factory.StartNew(() => successor(task));
			}).Unwrap();
		}

		public static Task<T> FromResult<T>(T value)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(value);
			return tcs.Task;
		}

		private static Task<T> FromError<T>(Exception e)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetException(e);
			return tcs.Task;
		}

		private static Task<T> Cancelled<T>()
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetCanceled();
			return tcs.Task;
		}

		public static Task<HttpWebResponse> GetAsync(this HttpWebRequest request)
		{
			return Task.Factory.FromAsync(request.BeginGetResponse, iar => (HttpWebResponse)request.EndGetResponse(iar), null);
		}

		public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request)
		{
			return Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null);
		}

		public static Task<HttpWebResponse> GetAsync(string url)
		{
			return GetAsync(url, _ => { });
		}

		public static Task<HttpWebResponse> GetAsync(string url, Action<HttpWebRequest> requestPreparer)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			requestPreparer(request);
			return request.GetAsync();
		}

		public static Task<HttpWebResponse> PostAsync(string url)
		{
			return PostInternal(url, _ => { }, new Dictionary<string, string>());
		}

		public static Task<HttpWebResponse> PostAsync(string url, IDictionary<string, string> postData)
		{
			return PostInternal(url, _ => { }, postData);
		}

		public static Task<HttpWebResponse> PostAsync(string url, Action<WebRequest> requestPreparer, IDictionary<string, string> postData)
		{
			return PostInternal(url, requestPreparer, postData);
		}

		public static string ReadAsString(this HttpWebResponse response)
		{
			using (response)
			using (var stream = response.GetResponseStream())
			using (var reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}

		private static Task<HttpWebResponse> PostInternal(string url, Action<WebRequest> prepareFn, IDictionary<string, string> postData)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);

			prepareFn(request);

			var sb = new StringBuilder();
			foreach (var pair in postData)
			{
				if (sb.Length > 0) sb.Append("&");

				if (string.IsNullOrEmpty(pair.Value)) continue;

				sb.AppendFormat("{0}={1}", pair.Key, Uri.EscapeUriString(pair.Value));
			}

			var buffer = Encoding.UTF8.GetBytes(sb.ToString());

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = buffer.LongLength;

			return request.GetRequestStreamAsync()
				.Success(t => t.Result.Write(buffer, 0, buffer.Length))
				.Success(t => request.GetAsync())
				.Unwrap();
		}

		public static Task<string> DownloadJsonAsync(this string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Accept = MimeTypes.Json;
			return request.GetAsync()
				.ContinueWith(t => {
					try
					{
						return t.Result.ReadAsString();
					}
					catch (Exception ex)
					{
						Log.Error(ex);
						return "[]";
					}
				});
		}

		public static List<Task<string>> DownloadAllAsync(this IEnumerable<string> urls, string contentType, Action<HttpWebRequest, Uri> filter = null)
		{
			List<Task<string>> tasks = urls.ToList().Select(url => {
					var uri = new Uri(url);
					var request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Accept = MimeTypes.Json;
					if (filter != null)
					{
						filter(request, uri);
					}
					return request.GetAsync()
						.ContinueWith(t => {
							try
							{
								return t.Result.ReadAsString();
							}
							catch (Exception ex)
							{
								Log.Error(ex);
                                if (contentType == MimeTypes.Json) return "[]";
								throw;
							}
						});
				})
				.ToList();

			return tasks;
		}

		public static List<Task<string>> DownloadAllJsonAsync(this IEnumerable<string> urls)
		{
            return DownloadAllAsync(urls, MimeTypes.Json);
		}
	}
}


