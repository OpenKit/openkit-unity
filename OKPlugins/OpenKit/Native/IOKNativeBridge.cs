using System;

namespace OpenKit.Native
{
	public interface IOKNativeBridge
	{
		void SetAppKey(string appKey);
		void SetSecretKey(string secretKey);
		void SetEndpoint(string endpoint);
		
		void ShowLeaderboards();
		void ShowLeaderboard(int leaderboardID);
		void ShowLeaderboardsLandscapeOnly();
		void ShowLoginToOpenKit();
		void ShowLeaderboardLandscapeOnly(int leaderboardID);
		
		void SubmitScoreComponent(OKScoreSubmitComponent score);
		void SubmitAchievementScore(OKAchievementScore achievementScore);
		void LogoutCurrentUserFromOpenKit();
		OKUser GetCurrentUser();
		void GetFacebookFriendsList(OKNativeAsyncCall functionCall);
		void SetAchievementsEnabled(bool enabled);
		void SetLeaderboardListTag(String tag);
		void SetGoogleLoginEnabled(bool enabled);
	}
}

