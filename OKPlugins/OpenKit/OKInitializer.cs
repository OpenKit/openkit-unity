using System.Collections;
using System;
using UnityEngine;
using OpenKit;


public class OKInitializer : MonoBehaviour
{
	// Get your appKey and secretKey from the OpenKit developer dashboard.  The keys below
	// are for the openkit sample app.
	private string appKey = "zRn4FrBcWi6ntUmWnEwm";
	private string secretKey = "rjqQmuDZaO6JtLuW25XPB2D6P0jplBfmuuANCKuu";
	private string endpoint = "http://development.openkit.io";

	void Awake()
	{
		gameObject.name = "OpenKitPrefab";
		DontDestroyOnLoad(gameObject);

		OKManager.AppKey = appKey;
		OKManager.SecretKey = secretKey;
		OKManager.Endpoint = endpoint;

#if UNITY_ANDROID && !UNITY_EDITOR
		OKManager.InitializeAndroid();
#endif
	}

	// Forward native events to OKManager.  It will figure out what to do with them.
	private void NativeViewWillAppear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewWillAppear);
	}

	private void NativeViewDidAppear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewDidAppear);
	}

	private void NativeViewWillDisappear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewWillDisappear);
	}

	private void NativeViewDidDisappear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewDidDisappear);
	}

	public static void GetSocialScores(leaderboard)
	{

	}

}
