using System.Collections;
using UnityEngine;
using OpenKit;

public class OKInitializer : OKBaseInitializer
{
	void SetupOpenKit()
	{
		// Get your appKey and secretKey from the OpenKit developer dashboard.
		// The keys below are for the openkit sample app.
		string myAppKey = "zRn4FrBcWi6ntUmWnEwm";
		string mySecretKey = "rjqQmuDZaO6JtLuW25XPB2D6P0jplBfmuuANCKuu";
		string myEndpoint = "http://development.openkit.io";
		
		// You must call OKManager.Configure(..)
		// In a production game you will likely call OKManager.Configure(appKey,SecretKey) and use the default endpoint
		OKManager.Configure(myAppKey, mySecretKey, myEndpoint);

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
