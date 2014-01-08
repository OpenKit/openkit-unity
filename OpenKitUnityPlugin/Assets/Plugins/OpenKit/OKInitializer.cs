using System.Collections;
using UnityEngine;
using OpenKit;

public class OKInitializer : OKBaseInitializer
{
	void SetupOpenKit()
	{
		OKSettings.Load();
		// AppKey and SecretKey are set in the OpenKit menu
		OKManager.Configure(OKSettings.AppKey, OKSettings.AppSecretKey);

		// If you want to disable the achievements UI, uncomment the line below
		//OKManager.SetAchievementsEnabled(false);
		
		// If you want to display a LeaderboardListTag other than the default "v1" tag, then
		// uncomment this line and set the tag
		//OKManager.SetLeaderboardListTag("v1");
		
		// If you want to disable Google Login, uncomment the below line (Android only)
		//OKManager.SetGoogleLoginEnabled(false);
	}

	public override void Awake()
	{
		base.Awake();
		SetupOpenKit();
	}
}
