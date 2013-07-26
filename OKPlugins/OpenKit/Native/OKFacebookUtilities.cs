using System;
using OpenKit;
using UnityEngine;

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
		
		public static void getFacebookFriends(Action<bool,string> callback)
		{
			GameObject gameObject = new GameObject("OpenKitGetFBFriendsTempObject");
			OKFBFriendsRequest request = gameObject.AddComponent<OKFBFriendsRequest>();
			request.callFunction(callback);
		}
	}
}

