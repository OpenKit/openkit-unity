using System.Collections;
using UnityEngine;
using OpenKit;

public class OKInitializer : OKBaseInitializer
{
	// Get your appKey and secretKey from the OpenKit developer dashboard.
	// The keys below are for the openkit sample app.
	new protected void SetupOpenKit()
	{
		base.SetupOpenKit();

		OKManager.AppKey = "zRn4FrBcWi6ntUmWnEwm";
		OKManager.SecretKey = "rjqQmuDZaO6JtLuW25XPB2D6P0jplBfmuuANCKuu";
		OKManager.Endpoint = "http://development.openkit.io";
		
		// If you want to disable the achievements UI, uncomment the line below
		//OKManager.SetAchievementsEnabled(false);
		
		// If you want to display a LeaderboardListTag other than the default "v1" tag, then
		// uncomment this line
		OKManager.SetLeaderboardListTag("v2");
	}

	void Awake()
	{
		SetupOpenKit();
	}
}
