using System;
using UnityEngine;

namespace OpenKit.Native
{
	public class OpenKitDummyObject : IOKNativeBridge
	{
		public OpenKitDummyObject ()
		{
		}

		public void Configure(string appKey, string secretKey, string endpoint) {}
		public void ShowLeaderboards() {}
		public void ShowLeaderboard(int leaderboardID) {}
		public void ShowLeaderboardLandscapeOnly(int leaderboardID) {}
		public void ShowLoginToOpenKit() {}
		public void SubmitScoreComponent(OKScoreSubmitComponent score) {
			score.scoreSubmissionFailed("Can't submit scores from Unity editor, native only");
		}
		public void ShowLoginToOpenKit(OKNativeAsyncCall functionCall) {}
		public void SubmitAchievementScore(OKAchievementScore achievementScore) {}
		public OKUser GetCurrentUser() {return null;}
		public void LogoutCurrentUserFromOpenKit() {}
		public void ShowLeaderboardsLandscapeOnly() {}
		public void GetFacebookFriendsList(OKNativeAsyncCall functionCall) {}
		public void SetAchievementsEnabled(bool enabled) {}
		public void SetLeaderboardListTag(String tag) {}
		public void SetGoogleLoginEnabled(bool enabled) {}
	}
}

