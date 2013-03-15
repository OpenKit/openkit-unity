using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace OpenKit
{
	public class OKCloud
	{
		public OKCloud() {}
		
		// CHANGEME:
		public static OKUser GetUser()
		{
			return OKManager.GetCurrentUser();
		}
		
		#region Public API
		public static void Set(object o, string key, Action<object, OKCloudException> handler) 
		{
			new OKCloud().mSet(o, key, handler);
		}
		
		public static void Get(string key, Action<JSONObject, OKCloudException> handler)
		{
			new OKCloud().mGet(key, handler);
		}
		#endregion
		

		#region Private
		public void mSet(object o, String key, Action<object, OKCloudException> handler)
		{
			OKUser u = GetUser();
			if (u == null)
				throw new Exception("You need a user to perform cloud set.");
			
			string objRep = "";
			objRep += JSONObjectExt.encode(o);
			
			Dictionary<string, string> reqParams = new Dictionary<string, string>();
			reqParams.Add("user_id", u.OKUserID.ToString());
			reqParams.Add("field_key", key);
			reqParams.Add("field_value", objRep);
			
			OKCloudAsyncRequest req = new OKCloudAsyncRequest("developer_data", "POST", reqParams);
			req.performWithCompletionHandler((string response, OKCloudException e) => {
				handler(o, e);
			});
		}
		
		public void mGet(String key, Action<JSONObject, OKCloudException> handler)
		{
			OKUser u = GetUser();
			if (u == null)
				throw new Exception("You need a user to perform cloud get.");

			Dictionary<string, string> reqParams = new Dictionary<string, string>();
			reqParams.Add("user_id", u.OKUserID.ToString());

			string path = string.Format("developer_data/{0}", key);
			OKCloudAsyncRequest req = new OKCloudAsyncRequest(path, "GET", reqParams);
			req.performWithCompletionHandler((string response, OKCloudException e) => {
				JSONObject jsonObj = JSONObjectExt.decode(response);
				handler(jsonObj.GetField(key), e);
			});
		}
		#endregion
	}
}

