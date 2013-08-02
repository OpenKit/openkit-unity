using System;
using OpenKit;
using UnityEngine;
using System.Collections.Generic;

namespace OpenKit
{
	public class OKFBFriendsRequest : OKNativeAsyncCall
	{
		public override void callNativeFunction(OKNativeAsyncCall dynamicObject)
		{
			OKManagerImpl.Instance.getFacebookFriendsList(dynamicObject);
		}

	}

	public class OKFacebookUtilities
	{
		public OKFacebookUtilities ()
		{
		}

		public static void getFacebookFriendsList(Action<List<string>,OKException> callback)
		{
			getFacebookFriendsFromNative((bool didSucceed, string result) => {
				if(didSucceed) {
					List<string> fbFriendsList = parseListOfFBFriendsIntoArray(result);
					callback(fbFriendsList,null);
				} else {
					callback(null, new OKException(result));
				}
			});
		}


		private static void getFacebookFriendsFromNative(Action<bool,string> callback)
		{
			GameObject gameObject = new GameObject("OpenKitGetFBFriendsTempObject");
			OKFBFriendsRequest friendsRequest = gameObject.AddComponent<OKFBFriendsRequest>();
			friendsRequest.callFunction(callback);
		}

		private static List<string> parseListOfFBFriendsIntoArray(string fbFriendsList)
		{
			return new List<string>(fbFriendsList.Split(','));
		}
	}
}

