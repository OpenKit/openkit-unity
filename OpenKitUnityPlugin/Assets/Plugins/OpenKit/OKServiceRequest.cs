using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using RestSharp;
using RestSharp.Authenticators;


namespace OpenKit
{
	using RequestParams   = System.Collections.Generic.Dictionary<string, object>;
	using RequestHandler  = OKRequestHandler<OKServiceResponse>;

	public enum OKRestVerb
	{
		Get = 0,
		Post,
		Put
	}

	public class OKServiceResponse : OKIOResponse
	{
		public JSONObject Obj;
	}


	public class OKServiceRequest
	{
		private static string _host      = "api.openkit.lan";
		private static   int? _port      = 3000;
		private static string _version   = "v1";
		private static string _appKey    = "Nysj4cQIMhg7G3SAsdzz";
		private static string _secretKey = "BndtKmjcwkuUcrlvummkWP3YPZTnDZpsNA3xUJ7H";

		public static string Host      { get { return _host; }      set { __serviceClient = null; _host = value; } }
		public static   int? Port      { get { return _port; }      set { __serviceClient = null; _port = value; } }
		public static string Version   { get { return _version; }   set { __serviceClient = null; _version = value; } }
		public static string AppKey    { get { return _appKey; }    set { __serviceClient = null; _appKey = value; } }
		public static string SecretKey { get { return _secretKey; } set { __serviceClient = null; _secretKey = value; } }


		public static OKServiceRequest Get(string path, RequestParams parameters, RequestHandler handler)
		{
			OKServiceRequest req = new OKServiceRequest(path, parameters, OKRestVerb.Get);
			req.Perform(handler);
			return req;
		}

		public static OKServiceRequest Post(string path, RequestParams parameters, RequestHandler handler)
		{
			OKServiceRequest req = new OKServiceRequest(path, parameters, OKRestVerb.Post);
			req.Perform(handler);
			return req;
		}

		public static OKServiceRequest MultipartPost(string path, RequestParams parameters, OKUploadBuffer upload, RequestHandler handler)
		{
			OKServiceRequest req = new OKServiceRequest(path, parameters, OKRestVerb.Post);
			req.Upload = upload;
			req.Perform(handler);
			return req;
		}

		public static OKServiceRequest Put(string path, RequestParams parameters, RequestHandler handler)
		{
			OKServiceRequest req = new OKServiceRequest(path, parameters, OKRestVerb.Put);
			req.Perform(handler);
			return req;
		}


		private string         _path;
		private RequestParams  _params;
		private OKRestVerb     _verb;
		private RestClient     _httpClient;
		private RequestHandler _didFinish;
		private OKUploadBuffer _upload;
		private RestRequestAsyncHandle  _killSwitch;

		public OKUploadBuffer Upload { set { _upload = value; } }
		public bool Cancelled { get; private set; }

		protected OKServiceRequest(string path, RequestParams parameters, OKRestVerb verb)
		{
			_path = path;
			_params = parameters;
			_verb = verb;
			_httpClient = GetServiceClient();
		}

		protected void Perform(RequestHandler handler)
		{
			_didFinish = handler;
			SynchronizationContext c = OKCtx.Ctx;
			if (c == null)
				DoPerform(this);
			else
				c.Post(DoPerform, this);
		}

		protected void SynchronizedPerform()
		{
			_killSwitch = _httpClient.ExecuteAsync(GetHttpRequest(), response => {
				var y = new OKServiceResponse();

				if (response.ResponseStatus == ResponseStatus.Aborted)
					y.Status = OKIOStatus.Cancelled;
				else {
					switch (response.StatusCode) {
						case HttpStatusCode.OK:
							y.Status = OKIOStatus.Succeeded;
							y.Obj = JSONObjectExt.decode(response.Content);
							break;
						case HttpStatusCode.Forbidden:
							y.Status = OKIOStatus.FailedWithError;
							y.Err = new OKException("Forbidden: Verify that your key and secret are correct.");
							break;
						default:
							y.Status = OKIOStatus.FailedWithError;
							y.Err = new OKException("OKServiceRequest failed.  Response body: " + response.Content);
							break;
					}
				}

				SynchronizationContext c = OKCtx.Ctx;
				if (c == null)
					_didFinish(y);
				else
					c.Post(s => _didFinish(y), null);

				_killSwitch = null;
			});
		}


		public void Cancel()
		{
			SynchronizationContext c = OKCtx.Ctx;
			if (c == null)
				DoCancel(null);
			else
				c.Post(DoCancel, null);
		}


		private void SynchronizedCancel()
		{
			if (!Cancelled) {
				Cancelled = true;
				if (_killSwitch != null) {
					_killSwitch.Abort();
				}
			}
		}


		public RestRequest GetHttpRequest()
		{
			RestRequest httpRequest = null;
			switch (_verb) {
				case OKRestVerb.Get:
					httpRequest = BuildGetRequest(_path, _params);
					break;
				case OKRestVerb.Post:
					httpRequest = (_upload == null) ? BuildPostRequest(_path, _params) : BuildMultipartPostRequest(_path, _params, _upload);
					break;
				case OKRestVerb.Put:
					httpRequest = BuildPutRequest(_path, _params);
					break;
				default:
					OKLog.Error("Doing it wrong!");
					break;
			}
			return httpRequest;
		}

		private static IEnumerable FormPairs(Dictionary<string, object> parameters)
		{
			yield return FormPair.Each(parameters);
		}

		private static RestRequest BuildGetRequest(string path, RequestParams parameters)
		{
			RestRequest request = new RestRequest(path, Method.GET);
			request.AddHeader("Accepts", "application/json");
			if (parameters != null) {
				foreach (FormPair pair in FormPairs(parameters)) {
					request.AddParameter(pair.Name, pair.Value);
				}
			}
			return request;
		}


		private static RestRequest BuildPostRequest(string path, RequestParams parameters)
		{
			RestRequest request = new RestRequest(path, Method.POST);
			request.AddHeader("Accepts", "application/json");
			request.AddHeader("Content-Type", "application/json");
			request.AddParameter("application/json", JSONObjectExt.encode(parameters), ParameterType.RequestBody);
			return request;
		}


		private static RestRequest BuildMultipartPostRequest(string path, RequestParams parameters, OKUploadBuffer upBuff)
		{
			RestRequest request = new RestRequest(path, Method.POST);
			request.AddHeader("Accepts", "application/json");
			request.AddFile(upBuff.ParamName, upBuff.Bytes, upBuff.FileName);
			if (parameters != null) {
				foreach (FormPair pair in FormPairs(parameters)) {
					request.AddParameter(pair.Name, pair.Value);
				}
			}
			return request;
		}

		private static RestRequest BuildPutRequest(string path, RequestParams parameters)
		{
			RestRequest request = new RestRequest(path, Method.PUT);
			request.AddHeader("Accepts", "application/json");
			request.AddHeader("Content-Type", "application/json");
			request.AddParameter("application/json", JSONObjectExt.encode(parameters), ParameterType.RequestBody);
			return request;
		}


		public static string GetBaseUrl()
		{
			if (Port == null || Port == 80) {
				return String.Format("http://{0}/{1}", Host, Version);
			} else {
				return String.Format("http://{0}:{1}/{2}", Host, Port, Version);
			}
		}

		private static RestClient __serviceClient;
		private static RestClient GetServiceClient()
		{
			if (__serviceClient == null) {
				__serviceClient = new RestClient(GetBaseUrl());
				__serviceClient.Authenticator = OAuth1Authenticator.ForRequestToken(AppKey, SecretKey);
			}
			return __serviceClient;
		}

		private static void DoPerform(object state) { ((OKServiceRequest)state).SynchronizedPerform(); }
		private static void DoCancel(object state) { ((OKServiceRequest)state).SynchronizedCancel(); }
	}
}

