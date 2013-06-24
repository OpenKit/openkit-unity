#if UNITY_IOS
using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace OpenKit.Native
{
	public class OpenKitIOS : IOKNativeBridge
	{
		public const string OK_IPHONE_DLL = "__Internal";
		
		[DllImport (OK_IPHONE_DLL)]
        public static extern void OKBridgeSetAppKey(string appKey);
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeSetEndpoint(string endpoint);
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLeaderboards();
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLoginUI();
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeSubmitScoreWithGameCenter(Int64 scoreValue, int leaderboardID, int metadata, string displayString, string gameObjectName, string gamecenterLeaderboardID);
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeSubmitScore(Int64 scoreValue, int leaderboardID, int metadata, string displayString, string gameObjectName);
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern int OKBridgeGetCurrentUserOKID();
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern string OKBridgeGetCurrentUserNick();
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern long OKBridgeGetCurrentUserFBID();
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern long OKBridgeGetCurrentUserTwitterID();
			
		public OpenKitIOS() {}
		
		public void setAppKey(string appKey)
		{
			OKBridgeSetAppKey(appKey);
		}
		
		public void setEndpoint(string endpoint)
		{
			OKBridgeSetEndpoint(endpoint);
		}
		
		public void showLeaderboards()
		{
			OKBridgeShowLeaderboards();
		}
		
		public void showLoginToOpenKit()
		{
			OKBridgeShowLoginUI();
		}
		
		public void submitScore(OKScore score)
		{
			if(string.IsNullOrEmpty(score.gameCenterLeaderboardCategory)) {
				Debug.Log("Submitting score to OpenKit");
				OKBridgeSubmitScore(score.scoreValue, score.OKLeaderboardID, score.metadata, score.displayString, score.GetCallbackGameObjectName());
			} else {
				Debug.Log("Submitting score to OpenKit & Gamecenter");
				OKBridgeSubmitScoreWithGameCenter(score.scoreValue, score.OKLeaderboardID, score.metadata, score.displayString, score.GetCallbackGameObjectName(), score.gameCenterLeaderboardCategory);
			}
		}
		
		public void submitAchievementScore(OKAchievementScore achievementScore)
		{
			OpenKit.OKLog.Error("Submit achievement score is not yet implemented on iOS");
		}
		
		public OKUser getCurrentUser()
		{	
			int okID = OKBridgeGetCurrentUserOKID();
			OKLog.Info("!!!!!! Got user ID" + okID);
	
			if(okID == 0)
				return null;
			else {
				OKUser user = new OKUser();	
				user.OKUserID = okID;
				user.userNick = OKBridgeGetCurrentUserNick();
				user.FBUserID = OKBridgeGetCurrentUserFBID();
				user.twitterUserID = OKBridgeGetCurrentUserTwitterID();
				return user;
			}
		}
	}
}
#endif
