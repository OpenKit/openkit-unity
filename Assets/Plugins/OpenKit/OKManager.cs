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
		
		public static void ShowLoginToOpenKit()
		{
			OKManagerImpl.Instance.ShowLoginToOpenKit();
		}
		
		public static OKUser GetCurrentUser()
		{
			return OKManagerImpl.Instance.GetCurrentUser();
		}
				
		public static void SubmitScore(OKScore score)
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
		
		public static void LogoutCurrentUserFromOpenKit()
		{
			OKManagerImpl.Instance.LogoutCurrentUserFromOpenKit();
		}

		#endregion
		
		public OKManager() {}
	}
}
