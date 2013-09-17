using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace OpenKit
{
	public class OKCloud
	{
		public OKCloud() {}

		public static OKUser GetUser()
		{
			return OKManager.GetCurrentUser();
		}

		#region Public API
		public static void Set(object o, string key, Action<OKCloudException> handler)
		{
			new OKCloud().mSet(o, key, handler);
		}

		public static void Get(string key, Action<object, OKCloudException> handler)
		{
			new OKCloud().mGet(key, handler);
		}
		#endregion


		#region Private
		public void mSet(object o, String key, Action<OKCloudException> handler)
		{
			OKUser u = GetUser();
			if (u == null)
				throw new Exception("You need a user to perform cloud set.");

			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("user_id", u.OKUserID.ToString());
			reqParams.Add("field_key", key);
			reqParams.Add("field_value", o);
			OKCloudAsyncRequest.Post("/developer_data", reqParams, (JSONObject responseObj, OKCloudException e) => {
				if(e != null) {
					OKLog.Error("Async post failed with error " + e);
				}
				handler(e);
			});
		}

		public void mGet(String key, Action<object, OKCloudException> handler)
		{
			OKUser u = GetUser();
			if (u == null)
				throw new Exception("You need a user to perform cloud get.");

			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("user_id", u.OKUserID.ToString());

			string path = string.Format("/developer_data/{0}", key);
			OKCloudAsyncRequest.Get(path, reqParams, (JSONObject responseObj, OKCloudException e) => {
				JSONObject j = responseObj.GetField(key);
				object retObject = JSONObjectExt.DeJSONify(j);
				if(retObject == null){
					handler(null, new OKCloudException("Fail."));
				} else {
					handler(retObject, null);
				}
			});
		}
		#endregion
	}
}

