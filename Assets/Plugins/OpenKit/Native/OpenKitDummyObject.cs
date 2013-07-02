using System;
using UnityEngine;

namespace OpenKit.Native
{
	public class OpenKitDummyObject : IOKNativeBridge
	{
		public OpenKitDummyObject ()
		{
		}
		
		public void setAppKey(string appKey) {}
		public void setSecretKey(string secretKey) {}
		public void setEndpoint(string endpoint) {}
		public void showLeaderboards() {}
		public void showLoginToOpenKit() {}
		public void submitScore(OKScore score) {}
		public void submitAchievementScore(OKAchievementScore achievementScore) {}
		public OKUser getCurrentUser() {return null;}
		public void logoutCurrentUserFromOpenKit() {}
	}
}

