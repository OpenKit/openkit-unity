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
		
		public void Configure(string appKey, string secretKey, string endpoint)
		{
			OKAndroidPlugin.CallStatic("configure", appKey, secretKey, endpoint);
		}

		public void ShowLeaderboards()
		{
			OKAndroidPlugin.CallStatic("showLeaderboards");
		}
		
		public void ShowLeaderboard(int leaderboardID)
		{
			OKAndroidPlugin.CallStatic("showLeaderboard",leaderboardID);
		}
		
		public void ShowLeaderboardLandscapeOnly(int leaderboardID)
		{
			Debug.Log("ShowLeaderboardLandscapeOnly on Android simply displays leaderboards. To restrict them to landscape only you need to modify AndroidManifest.xml");
			OKAndroidPlugin.CallStatic("showLeaderboard",leaderboardID);
		}

		public void ShowLeaderboardsLandscapeOnly()
		{
			Debug.Log("ShowLeaderboardsLandscapeOnly on Android simply displays leaderboards. To restrict them to landscape only you need to modify AndroidManifest.xml");
			OKAndroidPlugin.CallStatic("showLeaderboards");
		}

		public void ShowLoginToOpenKit()
		{
			OKAndroidPlugin.CallStatic("showLoginUI");
		}


		public void SubmitScoreComponent(OKScoreSubmitComponent score)
		{
			if(score.displayString == null) {
				//Set the displayString to blank if it's null because you can't pass null strings to JNI functions
				score.displayString = "";
			}
			OKAndroidPlugin.CallStatic("submitScore", score.scoreValue, score.OKLeaderboardID, score.metadata, score.displayString, score.GetCallbackGameObjectName());
		}

		public void SubmitScore(OKScoreSubmitComponent score)
		{
			if(score.displayString == null) {
				//Set the displayString to blank if it's null because you can't pass null strings to JNI functions
				score.displayString = "";
			}
			OKAndroidPlugin.CallStatic("submitScore", score.scoreValue, score.OKLeaderboardID, score.metadata, score.displayString, score.GetCallbackGameObjectName());
		}

		public void SubmitAchievementScore(OKAchievementScore achievementScore)
		{
			OKAndroidPlugin.CallStatic("submitAchievementScore", achievementScore.progress, achievementScore.OKAchievementID, achievementScore.GetCallbackGameObjectName());
		}

		public OKUser GetCurrentUser()
		{
			int okID = OKAndroidPlugin.CallStatic<int>("getCurrentUserOKID");

			if(okID == 0)
				return null;
			else {
				OKUser user = new OKUser();
				user.OKUserID = okID;
				user.userNick = OKAndroidPlugin.CallStatic<string>("getCurrentUserNick");
				user.FBUserID = OKAndroidPlugin.CallStatic<long>("getCurrentUserFBID");
				return user;
			}
		}

		public void LogoutCurrentUserFromOpenKit()
		{
			OKAndroidPlugin.CallStatic("logoutOfOpenKit");
		}

		public void GetFacebookFriendsList(OKNativeAsyncCall functionCall)
		{
			OKAndroidPlugin.CallStatic("getFacebookFriendsList", functionCall.GetCallbackGameObjectName());
		}
		
		public void SetAchievementsEnabled(bool enabled)
		{
			OKAndroidPlugin.CallStatic("setAchievementsEnabled",enabled);
		}
		
		public void SetLeaderboardListTag(String tag) 
		{
			OKAndroidPlugin.CallStatic("setLeaderboardListTag",tag);
		}
		public void SetGoogleLoginEnabled(bool enabled) 
		{
			OKAndroidPlugin.CallStatic("setGoogleLoginEnabled",enabled);
		}
	}
}
#endif
