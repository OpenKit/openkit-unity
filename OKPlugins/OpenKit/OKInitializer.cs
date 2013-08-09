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

		OKManager.AppKey = "D3iyQTsqRNF8nZWgsOpa";
		OKManager.SecretKey = "T6Y3yllYH5QusVpWo6wcMtURr4jIhKdMgpr2L5nd";
		OKManager.Endpoint = "http://192.168.1.100:3000";
	}

	void Awake()
	{
		SetupOpenKit();
	}
}
