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
		public void submitScoreComponent(OKScoreSubmitComponent score) {
			score.scoreSubmissionFailed("Can't submit scores from Unity editor, native only");
		}
		public void submitAchievementScore(OKAchievementScore achievementScore) {}
		public OKUser getCurrentUser() {return null;}
		public void logoutCurrentUserFromOpenKit() {}
		public void showLeaderboardsLandscapeOnly() {}
		public void getFacebookFriendsList(OKNativeAsyncCall functionCall) {}
	}
}

