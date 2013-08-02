using UnityEngine;
using System;
using System.Collections;
using OpenKit.Native;

namespace OpenKit
{
	public class OKManager
	{
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		#region Public API
		/////////////////////////////////////////////////////////////////////////////////////////////////////
		public static event EventHandler ViewWillAppear;
		public static event EventHandler ViewWillDisappear;
		public static event EventHandler ViewDidAppear;
		public static event EventHandler ViewDidDisappear;

		public static string AppKey
		{
			get { return OKManagerImpl.Instance.AppKey; }
			set { OKManagerImpl.Instance.AppKey = value; }
		}

		public static string SecretKey
		{
			get { return OKManagerImpl.Instance.SecretKey; }
			set { OKManagerImpl.Instance.SecretKey = value;}
		}

		public static string Endpoint
		{
			get { return OKManagerImpl.Instance.Endpoint; }
			set { OKManagerImpl.Instance.Endpoint = value; }
		}

		public static void ShowLeaderboards()
		{
			OKManagerImpl.Instance.ShowLeaderboards();
		}

		public static void ShowLeaderboardsLandscapeOnly()
		{
			OKManagerImpl.Instance.ShowLeaderboardsLandscapeOnly();
		}

		public static void ShowLoginToOpenKit()
		{
			OKManagerImpl.Instance.ShowLoginToOpenKit();
		}

		public static OKUser GetCurrentUser()
		{
			return OKManagerImpl.Instance.GetCurrentUser();
		}

		public static void SubmitScore(OKScoreSubmitComponent score)
		{
			OKManagerImpl.Instance.SubmitScore(score);
		}

		public static void SubmitAchievementScore(OKAchievementScore achievementScore)
		{
			OKManagerImpl.Instance.SubmitAchievementScore(achievementScore);
		}

		public static void authenticateGameCenterLocalPlayer()
		{
			OKManagerImpl.Instance.AuthenticateLocalPlayerWithGameCenter();
		}

		public static void AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary()
		{
			OKManagerImpl.Instance.AuthenticateLocalPlayerWithGameCenterAndShowGameCenterUIIfNecessary();
		}

		public static void LogoutCurrentUserFromOpenKit()
		{
			OKManagerImpl.Instance.LogoutCurrentUserFromOpenKit();
		}

		public static bool IsEnabled()
		{
			return OKManagerImpl.Instance.IsEnabled();
		}

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

		public static void InitializeAndroid()
		{
			OKManagerImpl.Instance.InitializeAndroid();
		}

		#endregion

		public OKManager() {}
	}
}
