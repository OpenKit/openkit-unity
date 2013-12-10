using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace OpenKit.Native
{
	public class OpenKitIOS : IOKNativeBridge
	{
		public const string OK_IPHONE_DLL = "__Internal";

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeConfigureOpenKit(string appKey, string secretKey, string endpoint);
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeSetLeaderboardListTag(string tag);

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLeaderboards();

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLeaderboardsLandscapeOnly();
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLeaderboardID(int leaderboardID);
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLeaderboardIDWithLandscapeOnly(int leaderboardID, bool landscapeOnly);

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLoginUI();

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeShowLoginUIWithBlock(string gameObjectName);

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeSubmitScoreWithGameCenter(Int64 scoreValue, int leaderboardID, int metadata, string displayString, string gameObjectName, string gamecenterLeaderboardID);

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeSubmitScore(Int64 scoreValue, int leaderboardID, int metadata, string displayString, string gameObjectName);

		[DllImport (OK_IPHONE_DLL)]
		public static extern int OKBridgeGetCurrentUserOKID();

		[DllImport (OK_IPHONE_DLL)]
		public static extern string OKBridgeGetCurrentUserNick();

		[DllImport (OK_IPHONE_DLL)]
		public static extern string OKBridgeGetCurrentUserFBID();

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeAuthenticateLocalPlayerWithGameCenter();

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeAuthenticateLocalPlayerWithGameCenterAndShowUIIfNecessary();

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeLogoutCurrentUserFromOpenKit();

		[DllImport (OK_IPHONE_DLL)]
		public static extern void OKBridgeGetFacebookFriends(string gameObjectName);
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern bool OKBridgeIsPlayerAuthenticatedWithGameCenter();
		
		[DllImport (OK_IPHONE_DLL)]
		public static extern bool OKBridgeIsCurrentUserAuthenticated();

		[DllImport (OK_IPHONE_DLL)]
		public static extern bool OKBridgeIsFBSessionOpen();
		

		public OpenKitIOS() {}
		
		public void Configure(string appKey, string secretKey, string endpoint) 
		{
			OKBridgeConfigureOpenKit(appKey, secretKey, endpoint);
		}

		public void ShowLeaderboards()
		{
			OKBridgeShowLeaderboards();
		}
		
		public void ShowLeaderboard(int leaderboardID) 
		{
			OKBridgeShowLeaderboardID(leaderboardID);
		}
		
		public void ShowLeaderboardLandscapeOnly(int leaderboardID)
		{
			OKBridgeShowLeaderboardIDWithLandscapeOnly(leaderboardID, true);
		}

		public void ShowLeaderboardsLandscapeOnly()
		{
			OKBridgeShowLeaderboardsLandscapeOnly();
		}

		public void ShowLoginToOpenKit()
		{
			OKBridgeShowLoginUI();
		}
		
		
		public bool IsPlayerAuthenticatedWithGameCenter()
		{
			return OKBridgeIsPlayerAuthenticatedWithGameCenter();
		}
		
		public bool IsCurrentUserAuthenticated()
		{
			return OKBridgeIsCurrentUserAuthenticated();
		}

		public bool IsFBSessionOpen()
		{
			return OKBridgeIsFBSessionOpen();
		}

		public void AuthenticateLocalPlayerToGC()
		{
			OKBridgeAuthenticateLocalPlayerWithGameCenter();
		}

		public void AuthenticateLocalPlayerToGCAndShowUIIfNecessary()
		{
			OKBridgeAuthenticateLocalPlayerWithGameCenterAndShowUIIfNecessary();
		}

		public void SubmitScoreComponent(OKScoreSubmitComponent score)
		{
			if(string.IsNullOrEmpty(score.gameCenterLeaderboardCategory)) {
				Debug.Log("Submitting score to OpenKit");
				OKBridgeSubmitScore(score.scoreValue, score.OKLeaderboardID, score.metadata, score.displayString, score.GetCallbackGameObjectName());
			} else {
				Debug.Log("Submitting score to OpenKit & Gamecenter");
				OKBridgeSubmitScoreWithGameCenter(score.scoreValue, score.OKLeaderboardID, score.metadata, score.displayString, score.GetCallbackGameObjectName(), score.gameCenterLeaderboardCategory);
			}
		}
		public void SubmitAchievementScore(OKAchievementScore achievementScore)
		{
			OpenKit.OKLog.Error("Submit achievement score is not yet implemented on iOS");
		}

		public OKUser GetCurrentUser()
		{
			int okID = OKBridgeGetCurrentUserOKID();
			OKLog.Info("Current openkit user id: " + okID);

			if(okID == 0)
				return null;
			else {
				OKUser user = new OKUser();
				user.OKUserID = okID;
				user.UserNick = OKBridgeGetCurrentUserNick();
				user.FBUserID = OKBridgeGetCurrentUserFBID();
				
				OKLog.Info("Current user: " + user);
				return user;
			}
		}

		public void LogoutCurrentUserFromOpenKit()
		{
			OKBridgeLogoutCurrentUserFromOpenKit();
		}

		public void GetFacebookFriendsList(OKNativeAsyncCall functionCall)
		{
			OKBridgeGetFacebookFriends(functionCall.GetCallbackGameObjectName());
		}

		public void ShowLoginToOpenKit(OKNativeAsyncCall functionCall)
		{
			//OKBridgeMethod with gameObjectName
			string gameObjName = functionCall.GetCallbackGameObjectName();
			OKBridgeShowLoginUIWithBlock(gameObjName);
		}
		
		public void SetAchievementsEnabled(bool enabled)
		{
			Debug.Log("OpenKit achievements not yet implemented on iOS, so SetAchievementsEnabled is also NYI");
		}
		
		public void SetLeaderboardListTag(String tag) 
		{
			OKBridgeSetLeaderboardListTag(tag);
		}
		public void SetGoogleLoginEnabled(bool enabled) 
		{
			Debug.Log("Google auth not supported on iOS yet, so SetGoogleAuthEnabled(boolean) does nothing on iOS");
		}
		
	}
}


