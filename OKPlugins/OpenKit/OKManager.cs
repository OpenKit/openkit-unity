using UnityEngine;
using System;
using System.Collections;
using OpenKit.Native;
using System.Threading;

namespace OpenKit
{
	public class OKManager
	{
		public const string OPENKIT_SDK_VERSION = "1.0.5";
		private const string DEFAULT_ENDPOINT = "http://api.openkit.io";

		private string _LeaderboardListTag = null;
		private static IOKNativeBridge nativeBridge = null;

		/////////////////////////////////////////////////////////////////////////////////////////////////////
		#region Singleton Implementation
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		// Utilizing singleton pattern (Not thread safe!  That should be ok).
		// http://msdn.microsoft.com/en-us/library/ff650316.aspx
		private static OKManager instance;
		public static OKManager Instance
		{
			get
			{
				if (instance == null) {
					instance = new OKManager();
				}
				return instance;
			}
		}

		public OKManager()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			nativeBridge = new OpenKitAndroid();
#elif UNITY_IPHONE && !UNITY_EDITOR
			nativeBridge = new OpenKitIOS();
#else
			nativeBridge = new OpenKitDummyObject();
#endif
			OKCtx.SetCtx(SynchronizationContext.Current);

			_endpoint = DEFAULT_ENDPOINT;
		}
		#endregion

		/////////////////////////////////////////////////////////////////////////////////////////////////////
		#region Public API
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		public static event EventHandler ViewWillAppear;
		public static event EventHandler ViewWillDisappear;
		public static event EventHandler ViewDidAppear;
		public static event EventHandler ViewDidDisappear;

		public static void Configure(string appKey, string secretKey, string endpoint)
		{
			OKManager.Instance._Configure(appKey, secretKey, endpoint);
		}

		public static void Configure(string appKey, string secretKey)
		{
			OKManager.Configure(appKey, secretKey, null);
		}

		public static string AppKey
		{
			get { return OKManager.Instance._AppKey; }
		}

		public static string SecretKey
		{
			get { return OKManager.Instance._SecretKey; }
		}

		public static string Endpoint
		{
			get { return OKManager.Instance._Endpoint; }
		}


		public static OKUser GetCurrentUser()
		{
			return OKManager.Instance._GetCurrentUser();
		}

		/* METHODS TO SHOW OPENKIT UI */

		public static void ShowLeaderboards()
		{
			OKManager.Instance._ShowLeaderboards();
		}

		public static void ShowLeaderboardsLandscapeOnly()
		{
			OKManager.Instance._ShowLeaderboardsLandscapeOnly();
		}

		public static void ShowLeaderboard(int leaderboardID)
		{
			OKManager.instance._ShowLeaderboard(leaderboardID);
		}

		public static void ShowLeaderboardLandscapeOnly(int aLeaderboardID)
		{
			OKManager.instance._ShowLeaderboardLandscapeOnly(aLeaderboardID);
		}

		public static void ShowLeaderboardsAndAchivements()
		{
			OKManager.instance._ShowLeaderboardsAndAchievements();
		}

		public static void ShowLoginToOpenKit()
		{
			OKManager.Instance._ShowLoginToOpenKit();
		}

		public static void ShowAchievements()
		{
			OKManager.Instance._ShowAchievements();
		}

		public static void ShowAchievementsLandscapeOnly()
		{
			OKManager.Instance._ShowAchievementsLandscapeOnly();
		}

		public static void ShowLoginToOpenKitWithDismissCallback(Action callback)
		{
			OKLoginRequest.ShowLoginUIWithCallback(callback);
		}

		public static void AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary()
		{
			OKManager.Instance._AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary();
		}

		/* end show UI methods region */

		public static void SubmitScore(OKScoreSubmitComponent score)
		{
			OKManager.Instance._SubmitScore(score);
		}

		public static void SubmitAchievementScore(OKAchievementScore achievementScore)
		{
			OKManager.Instance._SubmitAchievementScore(achievementScore);
		}

		public static void authenticateGameCenterLocalPlayer()
		{
			OKManager.Instance._AuthenticateLocalPlayerWithGameCenter();
		}

		public static void LogoutCurrentUserFromOpenKit()
		{
			OKManager.Instance._LogoutCurrentUserFromOpenKit();
		}
		
		public static bool IsPlayerAuthenticatedWithGameCenter()
		{
			return OKManager.instance._IsPlayerAuthenticatedWithGameCenter();
		}

		public static bool IsCurrentUserAuthenticated()
		{
			return OKManager.Instance._IsCurrentUserAuthenticated();
		}

		public static bool IsEnabled()
		{
			return OKManager.Instance._IsEnabled();
		}

		public static void GetFacebookFriendsList(OKNativeAsyncCall functionCall)
		{
			OKManager.Instance._GetFacebookFriendsList(functionCall);
		}

		public static void SetAchievementsEnabled(bool enabled)
		{
			OKManager.instance._SetAchievementsEnabled(enabled);
		}


		public static void SetLeaderboardListTag(String tag)
		{
			OKManager.instance._SetLeaderboardListTag(tag);
		}

		public string GetLeaderboardListTag()
		{
			return _LeaderboardListTag;
		}

		public static void SetGoogleLoginEnabled(bool enabled)
		{
			OKManager.instance._SetGoogleLoginEnabled(enabled);
		}

		public static bool IsFBSessionOpen()
		{
			return OKManager.instance._IsFBSessionOpen();
		}
		

		// Native events are forwarded here from OKBaseInitializer.  This makes
		// the API consistent for using OKManager as a configuration point
		// for OpenKit.  That is, developers can setup event handlers with:
		//
		//     OKManager.ViewWillAppear    += HandleViewWillAppear;
		//
		// And the method definition of HandleViewWillAppear will look like this:
		//
		//     static void HandleViewWillAppear(object sender, EventArgs e) {
		//       // Pause gameplay.
		//     }
		//
		public static void HandleNativeEvent(object sender, OKNativeEvent ev)
		{
			switch (ev) {
			case OKNativeEvent.viewWillAppear:
				if (ViewWillAppear != null)
					ViewWillAppear(sender, EventArgs.Empty);
				break;
			case OKNativeEvent.viewDidAppear:
				if (ViewDidAppear != null)
					ViewDidAppear(sender, EventArgs.Empty);
				break;
			case OKNativeEvent.viewWillDisappear:
				if (ViewWillDisappear != null)
					ViewWillDisappear(sender, EventArgs.Empty);
				break;
			case OKNativeEvent.viewDidDisappear:
				if (ViewDidDisappear != null)
					ViewDidDisappear(sender, EventArgs.Empty);
				break;
			default:
				break;
			}
		}

		#endregion


		/////////////////////////////////////////////////////////////////////////////////////////////////////
		#region Instance
		/////////////////////////////////////////////////////////////////////////////////////////////////////

		public void _Configure(string appKey, string secretKey, string endpoint)
		{
			_appKey = appKey;
			_secretKey = secretKey;
			if(endpoint != null) {
				_endpoint = endpoint;
			} else {
				_endpoint = DEFAULT_ENDPOINT;
			}

			OKLog.Info("OpenKit configured with endpoint: " + _endpoint);

			nativeBridge.Configure(_appKey, _secretKey, _endpoint);
		}

		private string _appKey;
		public string _AppKey
		{
			get { return _appKey; }
		}

		private string _secretKey;
		public string _SecretKey
		{
			get { return _secretKey; }
		}

		private string _endpoint;
		public string _Endpoint
		{
			get { return _endpoint; }
		}

		public void _ShowLeaderboards()
		{
			nativeBridge.ShowLeaderboards();
		}

		public void _ShowLeaderboardsLandscapeOnly()
		{
			nativeBridge.ShowLeaderboardsLandscapeOnly();
		}

		public void _ShowLoginToOpenKit()
		{
			nativeBridge.ShowLoginToOpenKit();
		}
		
		public void _ShowLoginToOpenKit(OKNativeAsyncCall functionCall)
		{
			nativeBridge.ShowLoginToOpenKit(functionCall);
		}
		
		public OKUser _GetCurrentUser()
		{
#if UNITY_EDITOR
			Debug.LogError("Can't get an OpenKit user in the Editor.");
			return null;
#else
			return nativeBridge.GetCurrentUser();
#endif
		}

		public void _SubmitScore(OKScoreSubmitComponent score)
		{
			nativeBridge.SubmitScoreComponent(score);
		}

		public void _SubmitAchievementScore(OKAchievementScore achievementScore)
		{
			nativeBridge.SubmitAchievementScore(achievementScore);
		}

		public void _LogoutCurrentUserFromOpenKit()
		{
			nativeBridge.LogoutCurrentUserFromOpenKit();
		}

		public bool _IsEnabled()
		{
			return !(nativeBridge is OpenKitDummyObject);
		}

		public void _AuthenticateLocalPlayerWithGameCenter()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			((OpenKitIOS)nativeBridge).AuthenticateLocalPlayerToGC();
#else
			OKLog.Info("AuthenticateLocalPlayerWithGameCenter ONLY supported on iOS");
#endif
		}

		public void _AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			((OpenKitIOS)nativeBridge).AuthenticateLocalPlayerToGCAndShowUIIfNecessary();
#else
			OKLog.Info("AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary ONLY supported on iOS");
#endif
		}

		public bool _IsPlayerAuthenticatedWithGameCenter()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			return ((OpenKitIOS)nativeBridge).IsPlayerAuthenticatedWithGameCenter();
#else
			OKLog.Info("_IsPlayerAuthenticatedWithGameCenter ONLY supported on iOS");
			return false;
#endif
		}

		public bool _IsCurrentUserAuthenticated()
		{
			return nativeBridge.IsCurrentUserAuthenticated();
		}

		public void _GetFacebookFriendsList(OKNativeAsyncCall functionCall)
		{
			nativeBridge.GetFacebookFriendsList(functionCall);
		}

		public bool _IsFBSessionOpen()
		{
			return nativeBridge.IsFBSessionOpen();
		}

		public void _SetAchievementsEnabled(bool enabled)
		{
			nativeBridge.SetAchievementsEnabled(enabled);
		}

		public void _ShowLeaderboard(int aLeaderboardID)
		{
			nativeBridge.ShowLeaderboard(aLeaderboardID);
		}

		public void _ShowLeaderboardsAndAchievements()
		{
			nativeBridge.ShowLeaderboardsAndAchievements();
		}

		public void _ShowLeaderboardLandscapeOnly(int aLeaderboardID)
		{
			nativeBridge.ShowLeaderboardLandscapeOnly(aLeaderboardID);
		}

		public void _SetLeaderboardListTag(String tag)
		{
			nativeBridge.SetLeaderboardListTag(tag);
		}

		public void _SetGoogleLoginEnabled(bool enabled)
		{
			nativeBridge.SetGoogleLoginEnabled(enabled);
		}

		public void _ShowAchievements()
		{
			nativeBridge.ShowAchievements();
		}

		public void _ShowAchievementsLandscapeOnly()
		{
			nativeBridge.ShowAchievementsLandscapeOnly();
		}



		#endregion


		/////////////////////////////////////////////////////////////////////////////////////////////////////
		#region Overrides
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		// Called when logging object.
		public override string ToString()
		{
			return string.Format("{0}, Endpoint: {1}", base.ToString(), Endpoint);
		}
		#endregion
	}
}
