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
		private static string endpoint;
		public static string GetEndpoint()
		{
			return OKManager.Endpoint;
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
			foreach(KeyValuePair<String,object> entry in requestParams) {
				request.AddParameter(entry.Key, entry.Value);
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
				} else {
					handler(null, new OKCloudException(response.ErrorMessage));
				}
			});
		}

		public static void Post(string relativePath, Dictionary<string, object>requestParams, Action<JSONObject, OKCloudException>handler)
		{
			RestRequest request = BuildPostRequest(relativePath, requestParams);
			Request(request, handler);
		}
		
		public static void Get(string relativePath, Dictionary<string, object>requestParams, Action<JSONObject, OKCloudException>handler)
		{
			RestRequest request = BuildGetRequest(relativePath, requestParams);
			Request(request, handler);
		}
	}       
}
