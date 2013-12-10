using System;

namespace OpenKit.Native
{
	public interface IOKNativeBridge
	{
		/* Settings */
		void Configure(string appKey, string secretKey, string endpoint);
		void SetAchievementsEnabled(bool enabled);
		void SetLeaderboardListTag(String tag);
		void SetGoogleLoginEnabled(bool enabled);
		
		/* Show UI methods*/
		void ShowLeaderboards();
		void ShowLeaderboard(int leaderboardID);
		void ShowLeaderboardsLandscapeOnly();
		void ShowLoginToOpenKit();
		void ShowLoginToOpenKit(OKNativeAsyncCall functionCall);
		void ShowLeaderboardLandscapeOnly(int leaderboardID);
		
		/* Native score submission related methods */
		void SubmitScoreComponent(OKScoreSubmitComponent score);
		void SubmitAchievementScore(OKAchievementScore achievementScore);
		void GetFacebookFriendsList(OKNativeAsyncCall functionCall);
		
		/* OKUser methods */
		void LogoutCurrentUserFromOpenKit();
		OKUser GetCurrentUser();
		bool IsCurrentUserAuthenticated();
		bool IsFBSessionOpen();
	}
}

