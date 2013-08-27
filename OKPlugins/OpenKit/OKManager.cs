using UnityEngine;
using System;
using System.Collections;
using OpenKit.Native;
using System.Threading;

namespace OpenKit
{
	public class OKManager
	{

		private const string DEFAULT_ENDPOINT = "http://stage.openkit.io";
		
		private string _LeaderboardListTag = null;
		
		// Synchronization
		private SynchronizationContext syncContext = null;
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

			syncContext = SynchronizationContext.Current;
			if(syncContext == null)
				OKLog.Info("SynchronizationContext.Current is null.");
			else
				OKLog.Info("SynchronizationContext is set.");

			nativeBridge.SetEndpoint(DEFAULT_ENDPOINT);
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

		public static string AppKey
		{
			get { return OKManager.Instance._AppKey; }
			set { OKManager.Instance._AppKey = value; }
		}

		public static string SecretKey
		{
			get { return OKManager.Instance._SecretKey; }
			set { OKManager.Instance._SecretKey = value;}
		}

		public static string Endpoint
		{
			get { return OKManager.Instance._Endpoint; }
			set { OKManager.Instance._Endpoint = value; }
		}

		public static void ShowLeaderboards()
		{
			OKManager.Instance._ShowLeaderboards();
		}

		public static void ShowLeaderboardsLandscapeOnly()
		{
			OKManager.Instance._ShowLeaderboardsLandscapeOnly();
		}

		public static void ShowLoginToOpenKit()
		{
			OKManager.Instance._ShowLoginToOpenKit();
		}

		public static OKUser GetCurrentUser()
		{
			return OKManager.Instance._GetCurrentUser();
		}

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

		public static void AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary()
		{
			OKManager.Instance._AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary();
		}

		public static void LogoutCurrentUserFromOpenKit()
		{
			OKManager.Instance._LogoutCurrentUserFromOpenKit();
		}

		public static bool IsEnabled()
		{
			return OKManager.Instance._IsEnabled();
		}

		public static void InitializeAndroid()
		{
			OKManager.Instance._InitializeAndroid();
		}

		public static void GetFacebookFriendsList(OKNativeAsyncCall functionCall)
		{
			OKManager.Instance._GetFacebookFriendsList(functionCall);
		}
		
		public static void SetAchievementsEnabled(bool enabled)
		{
			OKManager.instance._SetAchievementsEnabled(enabled);
		}
		
		public static void ShowLeaderboard(int leaderboardID)
		{
			OKManager.instance._ShowLeaderboard(leaderboardID);
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
		private string _appKey;
		public string _AppKey
		{
			get { return _appKey; }
			set
			{
				nativeBridge.SetAppKey(value);
				_appKey = value;
			}
		}

		private string _secretKey;
		public string _SecretKey
		{
			get { return _secretKey; }
			set
			{
				nativeBridge.SetSecretKey(value);
				_secretKey = value;
			}

		}

		private string _endpoint;
		public string _Endpoint
		{
			get { return _endpoint; }
			set
			{
				nativeBridge.SetEndpoint(value);
				_endpoint = value;
			}
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
			Debug.Log("AuthenticateLocalPlayerWithGameCenter ONLY supported on iOS");
#endif
		}

		public void _AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			((OpenKitIOS)nativeBridge).AuthenticateLocalPlayerToGCAndShowUIIfNecessary();
#else
			Debug.Log("AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary ONLY supported on iOS");
#endif
		}

		public void _InitializeAndroid()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			((OpenKitAndroid)nativeBridge).InitializeAndroid();
#endif
		}

		public void _GetFacebookFriendsList(OKNativeAsyncCall functionCall)
		{
			nativeBridge.GetFacebookFriendsList(functionCall);
		}
		
		public void _SetAchievementsEnabled(bool enabled)
		{
			nativeBridge.SetAchievementsEnabled(enabled);
		}
		
		public void _ShowLeaderboard(int leaderboardID)
		{
			nativeBridge.ShowLeaderboard(leaderboardID);
		}
		
		public void _SetLeaderboardListTag(String tag)
		{
			nativeBridge.SetLeaderboardListTag(tag);
		}
		
		public void _SetGoogleLoginEnabled(bool enabled)
		{
			nativeBridge.SetGoogleLoginEnabled(enabled);
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
