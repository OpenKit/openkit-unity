using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenKit;
using System;

public class DemoScene : MonoBehaviour {


	void Start()
	{
		// Get your app key and secret key from the OpenKit developer dashboard.
		OKManager.AppKey = "VwfMRAl5Gc4tirjw";
		OKManager.SecretKey = "v7iEWbTJrm1sqBeUBFll4RpRqXBIQuFlNtfA5XnS";
		OKManager.Endpoint = "http://stage.openkit.io";
		
		OKUser currentUser = OKManager.GetCurrentUser();
		if(currentUser != null) {
			Debug.Log("Logged into OpenKit as " + currentUser.userNick);
		} else {
			Debug.Log("Not logged into OpenKit");
		}

		// Authenticate the local player with GameCenter (iOS only).
		OKManager.authenticateGameCenterLocalPlayer();
	}


	void Update() {}


	void ShowLeaderboards()
	{
		OKManager.ShowLeaderboards();
	}


	void ShowLoginUI()
	{
		OKManager.ShowLoginToOpenKit();
	}


	// Notes about posting a score:
	//
	// If the user is not logged in, the score will not be submitted successfully.
	//
	// Metadata (optional) is stored and retrieved with each score.  It can be used
	// to save additional state information with each score.
	//
	// The display string can be used to append units or create a custom format
	// for your score to be displayed.  The score value, passed in constructor,
	// is only used for sorting scores on the backend (to determine which is best),
	// the display string is used for displaying scores in the UI.
	void SubmitSampleScore()
	{
		string scoreString = "" + DateTime.Now.Month;
		scoreString += DateTime.Now.Day;
		scoreString += DateTime.Now.Hour;
		scoreString += DateTime.Now.Minute;

		long scoreValue = long.Parse(scoreString);

		OKScore score = new OKScore(scoreValue, 84);
		score.gameCenterLeaderboardCategory = "openkitlevel3";
		score.displayString = score.scoreValue + " points";

		score.metadata = 1;

		score.submitScore((success, errorMessage) => {
			if (success) {
				Debug.Log("Score submitted successfully!");
			} else {
				Debug.Log("Score did not submit. Error: " + errorMessage);
			}
		});
	}


	void UnlockSampleAchievement()
	{
		var achievementProgress = 5;
		var achievementId = 3;
		OKAchievementScore achievementScore = new OKAchievementScore(achievementProgress, achievementId);
		achievementScore.submitAchievementScore((success, errorMessage) => {
			if (success) {
				Debug.Log("Achievement score/progress submitted successfully!");
			} else {
				Debug.Log("Achievement score/progress did not submit. Error: " + errorMessage);
			}
		});
	}


	void StoreSampleDictionary()
	{
		Dictionary<string, object> x = new Dictionary<string, object>();
		x.Add("prop1", "Foo!");
		x.Add("prop2", 99);

		ArrayList arr = new ArrayList();
		arr.Add("Hello");
		arr.Add(-99);
		x.Add("prop3", arr);

		OKCloud.Set(x, "keyDict", (OKCloudException err) =>	{
			if (err == null) {
				OKLog.Info("Stored Dictionary!");
			} else {
				OKLog.Info("Error storing dictionary: " + err);
			}
		});
	}


	void RetrieveSampleDictionary()
	{
		OKCloud.Get("keyDict", (object obj, OKCloudException err) =>
		{
			OKLog.Info("In get dictionary handler! Obj is of class: " + obj.GetType().Name);
			Dictionary<string,object> dict = (Dictionary<string,object>)obj;
			if (err == null) {
				OKLog.Info("Object for property1:\nclass: " + dict["prop1"].GetType().Name + "\nvalue: " + dict["prop1"]);
				OKLog.Info("Object for property2:\nclass: " + dict["prop2"].GetType().Name + "\nvalue: " + dict["prop2"]);
				OKLog.Info("Object for property3:\nclass: " + dict["prop3"].GetType().Name);
				ArrayList arr = (ArrayList)dict["prop3"];
				OKLog.Info("Elements of array:");
				OKLog.Info("Element 0, class: " + arr[0].GetType().Name + " value: " + arr[0]);
				OKLog.Info("Element 1, class: " + arr[1].GetType().Name + " value: " + arr[1]);
			} else {
				OKLog.Info("Error fetching dictionary: " + err);
			}
		});
	}


	static bool IsPortraitOrientation()
	{
		return ((Screen.orientation == ScreenOrientation.Portrait) ||
			    (Screen.orientation == ScreenOrientation.PortraitUpsideDown));
	}


	// Note that Screen.width and Screen.height change upon rotation.
	static Matrix4x4 GetScaleMatrix()
	{
		Matrix4x4 scaleMatrix;
		float w = (IsPortraitOrientation() ? 320.0f : 480.0f);
		float s = (float)(Screen.width / w);
		scaleMatrix = Matrix4x4.Scale(new Vector3(s, s, s));
		return scaleMatrix;
	}


	void OnGUI()
	{
		GUI.matrix = GetScaleMatrix();
		Rect area = (IsPortraitOrientation() ? new Rect(0, 0, 320, 480) : new Rect(0, 0, 480, 320));
		GUILayout.BeginArea(area);
		GUILayoutOption h = GUILayout.Height(40);

		GUILayout.Label("Testing OpenKit...");

		if(GUILayout.Button("Show Leaderboards & Achievements", h)) {
			ShowLeaderboards();
		}

		if(GUILayout.Button("Show Login UI", h)) {
			ShowLoginUI();
		}

		if(GUILayout.Button("Submit Score to Level 3 Leaderboard", h)) {
			SubmitSampleScore();
		}

		if(GUILayout.Button("Unlock Achievement", h)) {
			UnlockSampleAchievement();
		}

		if(GUILayout.Button("Store dictionary", h)) {
			StoreSampleDictionary();
		}

		if(GUILayout.Button("Retrieve Dictionary", h)) {
			RetrieveSampleDictionary();
		}

		GUILayout.EndArea();
	}
	
}
