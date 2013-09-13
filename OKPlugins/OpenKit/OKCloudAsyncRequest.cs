using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using OpenKit;
using RestSharp;
using RestSharp.Authenticators;


namespace OpenKit
{
	public class OKCloudAsyncRequest
	{
		private static string OPENKIT_SERVER_VERSION = "v1";
		
		private static string endpoint;
		public static string GetEndpoint()
		{
			string BaseEndpoint = OKManager.Endpoint;
			
			if(BaseEndpoint.EndsWith("/")) {
				return BaseEndpoint + OPENKIT_SERVER_VERSION;
			} else {
				return BaseEndpoint + "/" + OPENKIT_SERVER_VERSION;
			}	
		}

		public static string GetAppKey()
		{
			return OKManager.AppKey;
		}

		private static RestClient restClient;
		protected static RestClient GetRestClient()
		{
			if (restClient == null) {
				restClient = new RestClient(GetEndpoint());
				restClient.Authenticator = OAuth1Authenticator.ForRequestToken(OKManager.AppKey, OKManager.SecretKey);
			}
			return restClient;
		}

		private static RestRequest BuildMultiPartPostRequest(string relativePath, Dictionary<string, object>requestParams, OKUploadBuffer upBuff)
		{
			RestRequest request = new RestRequest(relativePath, Method.POST);
			request.AddHeader("Accepts", "application/json");
			request.AddFile(upBuff.ParamName, upBuff.Bytes, upBuff.FileName);

			// This only handles one level of nesting! Fix me.
			foreach (var p1 in requestParams) {
				var v = p1.Value;
				var k = p1.Key;
				if (v.GetType() == typeof(Dictionary<string, object>)) {
					foreach (var p2 in (Dictionary<string, object>)v) {
						string paramKey = String.Format("{0}[{1}]", k, p2.Key);
						request.AddParameter(paramKey, p2.Value);
					}
				} else {
					request.AddParameter(k, v);
				}
			}
			return request;
		}

		private static RestRequest BuildPostRequest(string relativePath, Dictionary<string, object>requestParams)
		{
			RestRequest request = new RestRequest(relativePath, Method.POST);
			request.AddHeader("Accepts", "application/json");
			request.AddHeader("Content-Type", "application/json");
			request.AddParameter("application/json", JSONObjectExt.encode(requestParams), ParameterType.RequestBody);
			return request;
		}

		private static RestRequest BuildGetRequest(string relativePath, Dictionary<string, object>requestParams)
		{
			RestRequest request = new RestRequest(relativePath, Method.GET);
			request.AddHeader("Accepts", "application/json");
			if(requestParams != null) {
				foreach(KeyValuePair<String,object> entry in requestParams) {
					request.AddParameter(entry.Key, entry.Value);
				}
			}
			return request;
		}

		public static void Request(RestRequest request, Action<JSONObject, OKCloudException>handler)
		{
			RestClient client = GetRestClient();
			client.ExecuteAsync(request, (response) => {
				if (response.StatusCode == System.Net.HttpStatusCode.OK) {
					JSONObject jsonObj = JSONObjectExt.decode(response.Content);
					handler(jsonObj, null);
				} else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) {
					handler(null, new OKCloudException("Forbidden: Verify that your key and secret are correct."));
				} else {
					handler(null, new OKCloudException("Async request failed with message: " + JSONObjectExt.decode(response.Content).GetField("message")));
				}
			});
		}

		public static void Post(string relativePath, Dictionary<string, object>requestParams, Action<JSONObject, OKCloudException>handler)
		{
			Post(relativePath, requestParams, null, handler);
		}

		public static void Post(string relativePath, Dictionary<string, object>requestParams, OKUploadBuffer upBuff, Action<JSONObject, OKCloudException>handler)
		{
			RestRequest request;
			if (upBuff == null) {
				request = BuildPostRequest(relativePath, requestParams);
			} else {
				request = BuildMultiPartPostRequest(relativePath, requestParams, upBuff);
			}

			Request(request, handler);
		}

		public static void Get(string relativePath, Dictionary<string, object>requestParams, Action<JSONObject, OKCloudException>handler)
		{
			RestRequest request = BuildGetRequest(relativePath, requestParams);
			Request(request, handler);
		}
	}
}
