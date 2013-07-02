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

		public void setSecretKey(string secretKey)
		{
			OKAndroidPlugin.CallStatic("setSecretKey", secretKey);
		}

		public void setAppKey(string appKey)
		{
			OKAndroidPlugin.CallStatic("setAppKey", appKey);
		}
		
		public void setEndpoint(string endpoint)
		{
			OKAndroidPlugin.CallStatic("setEndpoint", endpoint);
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
			if(score.displayString == null) {
				//Set the displayString to blank if it's null because you can't pass null strings to JNI functions
				score.displayString = "";
			}
			OKAndroidPlugin.CallStatic("submitScore", score.scoreValue, score.OKLeaderboardID, score.metadata, score.displayString, score.GetCallbackGameObjectName());
		}
		
		public void submitAchievementScore(OKAchievementScore achievementScore)
		{
			OKAndroidPlugin.CallStatic("submitAchievementScore", achievementScore.progress, achievementScore.OKAchievementID, achievementScore.GetCallbackGameObjectName());
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
		
		public void logoutCurrentUserFromOpenKit()
		{
			UnityEngine.Debug.Log("Logout of OpenKit not implented yet on Android");
			
		}
	}
}
#endif