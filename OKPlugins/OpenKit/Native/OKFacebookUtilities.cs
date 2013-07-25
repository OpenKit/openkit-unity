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
			OKFBFriendsRequest request = new  OKFBFriendsRequest();
			request.callFunction(callback);
		}
	}
}

