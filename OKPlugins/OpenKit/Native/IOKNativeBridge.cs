using System;

namespace OpenKit.Native
{
	public interface IOKNativeBridge
	{
		void SetAppKey(string appKey);
		void SetSecretKey(string secretKey);
		void SetEndpoint(string endpoint);
		void ShowLeaderboards();
		void ShowLeaderboardsLandscapeOnly();
		void ShowLoginToOpenKit();
		void SubmitScoreComponent(OKScoreSubmitComponent score);
		void SubmitAchievementScore(OKAchievementScore achievementScore);
		void LogoutCurrentUserFromOpenKit();
		OKUser GetCurrentUser();
		void GetFacebookFriendsList(OKNativeAsyncCall functionCall);
	}
}

