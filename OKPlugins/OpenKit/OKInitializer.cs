using System.Collections;
using UnityEngine;
using OpenKit;

public class OKInitializer : OKBaseInitializer
{
	void SetupOpenKit()
	{
		// Get your appKey and secretKey from the OpenKit developer dashboard.
		// The keys below are for the openkit sample app.
		string myAppKey = "BspfxiqMuYxNEotLeGLm";
		string mySecretKey = "2sHQOuqgwzocUdiTsTWzyQlOy1paswYLGjrdRWWf";
		
		// You must call OKManager.Configure(..)
		OKManager.Configure(myAppKey, mySecretKey);

		// If you want to disable the achievements UI, uncomment the line below
		//OKManager.SetAchievementsEnabled(false);
		
		// If you want to display a LeaderboardListTag other than the default "v1" tag, then
		// uncomment this line and set the tag
		//OKManager.SetLeaderboardListTag("v1");
		
		// If you want to disable Google Login, uncomment the below line (Android only)
		//OKManager.SetGoogleLoginEnabled(false);
	}

	void Awake()
	{
		SetupOpenKit();
	}
}
