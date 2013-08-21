using System;
using UnityEngine;

namespace OpenKit.Native
{
	public class OpenKitDummyObject : IOKNativeBridge
	{
		public OpenKitDummyObject ()
		{
		}

		public void SetAppKey(string appKey) {}
		public void SetSecretKey(string secretKey) {}
		public void SetEndpoint(string endpoint) {}
		public void ShowLeaderboards() {}
		public void ShowLeaderboard(int leaderboardID) {}
		public void ShowLoginToOpenKit() {}
		public void SubmitScoreComponent(OKScoreSubmitComponent score) {
			score.scoreSubmissionFailed("Can't submit scores from Unity editor, native only");
		}
		public void SubmitAchievementScore(OKAchievementScore achievementScore) {}
		public OKUser GetCurrentUser() {return null;}
		public void LogoutCurrentUserFromOpenKit() {}
		public void ShowLeaderboardsLandscapeOnly() {}
		public void GetFacebookFriendsList(OKNativeAsyncCall functionCall) {}
		public void SetAchievementsEnabled(bool enabled) {}
	}
}

