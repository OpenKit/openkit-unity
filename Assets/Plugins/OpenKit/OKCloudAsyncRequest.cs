using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using OpenKit;
using RestSharp;

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
			}
			return restClient;
		}
		
		protected static RestRequest BuildRestRequest(string relativePath, RestSharp.Method method, Dictionary<string, string>requestParams)
		{
			RestRequest request = new RestRequest(relativePath, method);
			request.AddHeader("Accepts", "application/json");
			request.AddParameter("app_key", GetAppKey());
			
			foreach(KeyValuePair<String,String> entry in requestParams) {
				request.AddParameter(entry.Key, entry.Value);
			}
			return request;
		}
			
		public string RelativePath { get; set; }
		public string RequestMethod { get; set; }
		public Dictionary<string, string> RequestParams { get; set; }
		
		protected RestSharp.Method RestSharpMethod
		{
			get
			{
				RestSharp.Method m = 0;
				switch (RequestMethod) {
				case "GET" : 
					m = Method.GET;
					break;
				case "POST" :
					m = Method.POST;
					break;
				default:
					throw new Exception("Wrong.");
				}
				return m;
			}
		}
		
		public OKCloudAsyncRequest(string relativePath, string requestMethod, Dictionary<string, string> requestParams)
		{
			this.RelativePath = relativePath;
			this.RequestMethod = requestMethod;
			this.RequestParams = requestParams;
		}
		
		public void performWithCompletionHandler(Action<string, OKCloudException> handler)
		{
			RestClient client = GetRestClient();
			RestRequest request = BuildRestRequest(RelativePath, RestSharpMethod, RequestParams);
			client.ExecuteAsync(request, (response) => {
				if (response.StatusCode == System.Net.HttpStatusCode.OK) {
					handler(response.Content, null);
				} else {
					var msg = String.Format("OpenKit Unity: Got a bad status code back from the server: {0}", response.StatusCode);
					handler(null, new OKCloudException(msg));
				}
			});
		} 
		
		#region Private
		#endregion
	
	}       
}
