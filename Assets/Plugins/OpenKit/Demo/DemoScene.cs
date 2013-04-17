using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenKit;

public class DemoScene : MonoBehaviour {

	// Use this for initialization
	void Start()
	{
		// This should be called at the beginning of your game. 
		// The app key below is taken from your OpenKit Application Key in the OpenKit dashboard.
		OKManager.AppKey = "VwfMRAl5Gc4tirjw";
		
		// This shows sample usage of checking whether the user is logged in 
		OKUser currentUser = OKManager.GetCurrentUser();
		if(currentUser != null)
			Debug.Log("Logged into OpenKit as " + currentUser.userNick);
		else
			Debug.Log("Not logged into OpenKit");
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
		
		
		if(GUI.Button(new Rect(30,10,400,100), "Show Leaderboards"))
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
			
			OKScore score = new OKScore(2134234, 6);
			
			// Set the displayString to include the units of the score
			score.displayString = score.scoreValue + " points";
			
			// Store some metadata in the score-- this is not used by OpenKit but is stored and returned with each score
			score.metadata = 1;
			
			score.submitScore(scoreSubmitHandler);
			
#endif
		}
		
		if(GUI.Button(new Rect(30,340,400,100), "Store dictionary"))
		{
			//Store a dictionary 
			
			ArrayList y = new ArrayList();
			y.Add("First element.");
			y.Add("Second!");
			
			Dictionary<string, object> x = new Dictionary<string, object>();
			x.Add("prop1", "YEAAAAAAH BUDDY.");
			x.Add("prop2", 99);
			x.Add("prop3", y);
			
			// Cloud store. 
			OKCloud.Set(x, "aKey", delegate(object obj, OKCloudException err)
			{
				if (err == null) {
					OKLog.Info("Stored object of type: " + obj.GetType().Name);
				} else {
					OKLog.Info("Error during store: " + err);
				}
			});
		}

		
		if(GUI.Button(new Rect(30,450,400,100), "Retrieve dictionary"))
		{
			//Retrieve the dictionary
			
			OKCloud.Get("aKey", delegate(JSONObject obj, OKCloudException err) 
			{
				if (err == null) {
					OKLog.Info("Retrieved object of type: " + obj.GetType().Name);
					OKLog.Info("Obj: " + obj);
					OKLog.Info("Can I get an element of an Array? " + obj.GetField("prop3")[1]);
				} else {
					OKLog.Info("Error during store: " + err);
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
}
