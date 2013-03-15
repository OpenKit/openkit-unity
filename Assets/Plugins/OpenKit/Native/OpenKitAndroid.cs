#if UNITY_ANDROID
using System;
using UnityEngine;
using OpenKit;

namespace OpenKit.Native
{
	public class OpenKitAndroid : IOKNativeBridge
	{
		private static AndroidJavaObject _OKAndroidPlugin;
		private static AndroidJavaObject OKAndroidPlugin
		{
			get
			{
				if (_OKAndroidPlugin == null)
				{
					_OKAndroidPlugin = new AndroidJavaObject("io.openkit.unity.android.UnityPlugin");
				}
				return _OKAndroidPlugin;
			}
		}
		
		public OpenKitAndroid ()
		{
		}
		
		public void setAppKey(string appKey)
		{
			OKAndroidPlugin.CallStatic("setAppKey", appKey);
		}
		
		public void setEndpoint(string endpoint)
		{
			OKLog.Error("Setting the Android endpoint is not supported yet!");
			// OKAndroidPlugin.CallStatic("setEndpoint", endpoint);
		}
		
		public void showLeaderboards()
		{
			OKAndroidPlugin.CallStatic("showLeaderboards");
		}
		
		public void showLoginToOpenKit()
		{
			OKAndroidPlugin.CallStatic("showLoginUI");
		}
		
		
		public void submitScore(OKScore score)
		{
			OKAndroidPlugin.CallStatic("submitScore", score.scoreValue, score.OKLeaderboardID, score.GetCallbackGameObjectName());
		}
		
		public OKUser getCurrentUser()
		{
			int okID = OKAndroidPlugin.CallStatic<int>("getCurrentUserOKID");
			
			if(okID == 0)
				return null;
			else {
				OKUser user = new OKUser();	
				user.OKUserID = okID;
				user.userNick = OKAndroidPlugin.CallStatic<string>("getCurrentUserNick");
				user.FBUserID = OKAndroidPlugin.CallStatic<long>("getCurrentUserFBID");
				user.twitterUserID = OKAndroidPlugin.CallStatic<long>("getCurrentUserTwitterID");
				return user;
			}
		}
	}
}
#endif