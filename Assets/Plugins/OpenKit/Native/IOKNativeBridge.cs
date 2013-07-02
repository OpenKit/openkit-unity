using System;

namespace OpenKit.Native
{
	public interface IOKNativeBridge
	{
		void setAppKey(string appKey);
		void setSecretKey(string secretKey);
		void setEndpoint(string endpoint);
		void showLeaderboards();
		void showLoginToOpenKit();
		void submitScore(OKScore score);
		void submitAchievementScore(OKAchievementScore achievementScore);
		void logoutCurrentUserFromOpenKit();
		OKUser getCurrentUser();
	}
}

