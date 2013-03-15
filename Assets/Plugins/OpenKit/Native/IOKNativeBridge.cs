using System;

namespace OpenKit.Native
{
	public interface IOKNativeBridge
	{
		void setAppKey(string appKey);
		void setEndpoint(string endpoint);
		void showLeaderboards();
		void showLoginToOpenKit();
		void submitScore(OKScore score);
		OKUser getCurrentUser();
	}
}

