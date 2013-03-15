#if UNITY_IOS
using System;
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
		public static extern void OKBridgeSubmitScore(int scoreValue, int leaderboardID, string gameObjectName);
		
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
			OKBridgeSubmitScore(score.scoreValue, score.OKLeaderboardID, score.GetCallbackGameObjectName());
		}
		
		public OKUser getCurrentUser()
		{
			int x = 5;
			
			OKLog.Info("Test user id: " + x);
			
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
