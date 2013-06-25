using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenKit;
using System;

public class DemoScene : MonoBehaviour {

	// Use this for initialization
	void Start()
	{
		// This should be called at the beginning of your game. 
		// The app key below is taken from your OpenKit Application Key in the OpenKit dashboard.
		OKManager.AppKey = "VwfMRAl5Gc4tirjw";
		
		//Set the endpoint to something other than the default
		OKManager.Endpoint = "http://stage.openkit.io";
	
		//TODO remove later
		OKManager.AppKey = "7jHqH0QcamsuvgMrlVZd";
		OKManager.Endpoint = "http://10.0.1.18:3000";
		
		// This shows sample usage of checking whether the user is logged in 
		OKUser currentUser = OKManager.GetCurrentUser();
		if(currentUser != null)
			Debug.Log("Logged into OpenKit as " + currentUser.userNick);
		else
			Debug.Log("Not logged into OpenKit");
		
		
		//Authenticate the local player with GameCenter (iOS only.. on Android this method does nothing)
		OKManager.authenticateGameCenterLocalPlayer();
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	void OnGUI() 
	{
		///Scale the button sizes for retina displays
		float screenScale = (float)(Screen.width / 480.0);
		Matrix4x4 scaledMatrix = Matrix4x4.Scale(new Vector3(screenScale, screenScale, screenScale));
		GUI.matrix = scaledMatrix;
		
		
		if(GUI.Button(new Rect(30,10,400,100), "Show Leaderboards & Achievements"))
		{
			// Show leaderboards. 
			// If the user is not logged into OpenKit, the login UI
			// will be shown ontop of the leaderboards
			OKManager.ShowLeaderboards();
		}
		
		if(GUI.Button(new Rect(30,120,400,100), "Show Login UI"))
		{
			// Show the OpenKit Login UI
			OKManager.ShowLoginToOpenKit();
		}
		
		if(GUI.Button(new Rect(30,230,400,100), "Submit Score to Level 3 Leaderboard"))
		{
						
#if !UNITY_EDITOR
			// Submit a score to a leaderboard, with a value of 2134 to leaderboard ID 4
			// If the user is not logged in, the score will not be submitted successfully
			
			string scoreString = "" + DateTime.Now.Month;
			scoreString += DateTime.Now.Day;
			scoreString += DateTime.Now.Hour;
			scoreString += DateTime.Now.Minute;
			
			long scoreValue = long.Parse(scoreString);
			
			OKScore score = new OKScore(scoreValue, 84);
			score.gameCenterLeaderboardCategory = "openkitlevel3";
			
			//TODO switch back to this
			//OKScore score = new OKScore(scoreValue, 4);
			
			// Set the displayString to include the units of the score	
			score.displayString = score.scoreValue + " points";
			
			// Store some metadata in the score-- this is not used by OpenKit but is stored and returned with each score
			score.metadata = 1;
			
			
			
			score.submitScore(scoreSubmitHandler);
			
#endif
		}
		
		if(GUI.Button(new Rect(30,340,400,100), "Unlock Achievement"))
		{
			//Unlock achievement by setting its progress for the current user
			// to 5. The achievement ID is pulled from the OpenKit dashboard,
			// and we know that the target goal of the achievement is also 5 which is set in the dashboard,
			// so this unlocks the achievement
			
			OKAchievementScore achievementScore = new OKAchievementScore(5, 3);
			
			achievementScore.submitAchievementScore(achievementScoreSubmitHandler);
		}
		
		if(GUI.Button(new Rect(30,450,400,100), "Store dictionary"))
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

		
		if(GUI.Button(new Rect(30,560,400,100), "Retrieve dictionary"))
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
	}
	
	// A sample delegate method for submitting a score
	void scoreSubmitHandler(bool success, string error)
	{
		if(success) {
			Debug.Log("Score submitted successfully!");
		}
		else {
			Debug.Log("Score did not submit. Error: " + error);
		}
	}
	
		// A sample delegate method for submitting an achievement score
	void achievementScoreSubmitHandler(bool success, string error)
	{
		if(success) {
			Debug.Log("Achievement score/progress submitted successfully!");
		}
		else {
			Debug.Log("Achievement score/progress did not submit. Error: " + error);
		}
	}
}
