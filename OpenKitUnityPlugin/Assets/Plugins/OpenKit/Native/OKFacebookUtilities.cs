using System;
using OpenKit;
using UnityEngine;
using System.Collections.Generic;

namespace OpenKit
{
	public class OKFBFriendsRequest : OKNativeAsyncCall
	{
		public override void CallNativeFunction(OKNativeAsyncCall dynamicObject)
		{
			OKManager.GetFacebookFriendsList(dynamicObject);
		}

	}

	public class OKFacebookUtilities
	{
		public OKFacebookUtilities ()
		{
		}

		public static void GetFacebookFriendsList(Action<List<string>,OKException> callback)
		{
			GetFacebookFriendsFromNative((bool didSucceed, string result) => {
				if(didSucceed) {
					string[] friends = result.Split(',');
					List<string> friendsList = new List<string>(friends);
					callback(friendsList,null);
				} else {
					callback(null, new OKException(result));
				}
			});
		}


		private static void GetFacebookFriendsFromNative(Action<bool,string> callback)
		{
			GameObject gameObject = new GameObject("OpenKitGetFBFriendsTempObject" + DateTime.Now.Ticks);
			OKFBFriendsRequest friendsRequest = gameObject.AddComponent<OKFBFriendsRequest>();
			friendsRequest.callFunction(callback);
		}
	}
}

