using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenKit;
using System;

public class OKDemoScene : MonoBehaviour {

	private static int SampleLeaderboardID = 17;


	void Setup()
	{
		// Authenticate the local player with GameCenter (iOS only).
		OKManager.authenticateGameCenterLocalPlayer();

		// Listen for native openkit view events.
		OKManager.ViewWillAppear    += ViewWillAppear;
		OKManager.ViewDidAppear     += ViewDidAppear;
		OKManager.ViewWillDisappear += ViewWillDisappear;
		OKManager.ViewDidDisappear  += ViewDidDisappear;
	}

	static void ViewWillAppear(object sender, EventArgs e) {
		Debug.Log("OK ViewWillAppear");
	}

	static void ViewWillDisappear(object sender, EventArgs e) {
		Debug.Log("OK ViewWillDisappear");
	}

	static void ViewDidAppear(object sender, EventArgs e) {
		Debug.Log("OK ViewDidAppear");
	}

	static void ViewDidDisappear(object sender, EventArgs e) {
		Debug.Log("OK ViewDidDisappear");
	}


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
		int lapTime = 5500;  // value in hundredths, 65 seconds.
		int total_sec = lapTime / 100;
		int total_min = total_sec / 60;
		int hour = total_min / 60;
		int min = total_min % 60;
		int sec = total_sec % 60;
		int hun = lapTime % 100;

		string scoreString = "" + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + "." + hun.ToString("00");

		OKScore score = new OKScore(lapTime, SampleLeaderboardID);
		score.gameCenterLeaderboardCategory = "openkitlevel3";
		score.displayString = scoreString + " seconds";

		Action<bool, string> nativeHandle = (success, errorMessage) => {
			if (success) {
				Debug.Log("Score submitted successfully!");
			} else {
				Debug.Log("Score did not submit. Error: " + errorMessage);
			}
		};

		Action<OKScore, OKException> defaultHandle = (retScore, err) => {
			if (err == null) {
				Debug.Log("Score submitted successfully: " + retScore.ToString());
			} else {
				Debug.Log("Score post failed: " + err.Message);
			}
		};

		bool dropToNative = false;
		if (dropToNative) {
			score.SubmitScoreNatively(nativeHandle);
		} else {
			score.MetadataBuffer = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x80 };
			score.SubmitScore(defaultHandle);
		}
	}


	void UnlockSampleAchievement()
	{
		var achievementProgress = 5;
		var achievementID = 3;
		OKAchievementScore achievementScore = new OKAchievementScore(achievementProgress, achievementID);
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

	// OKScore with meta document API (this stuff will make it into SDK in time):
	protected class OKGhostScoreLoader
	{
		private List<OKScore> _scores;
		public List<OKScore> Scores		{ get { return _scores; } }
		public delegate void GhostScoresDidLoadHandler(OKGhostScoreLoader sender);

		private GhostScoresDidLoadHandler _handler;
		private int LeaderboardID { get; set; }
		private List<OKScore> _pending;

		public OKGhostScoreLoader(int leaderboardID)
		{
			this.LeaderboardID = leaderboardID;
			_pending = new List<OKScore>();
		}

		// TODO: This should return a reference to an obj that can cancel all requests.
		public void ExecuteAsync(GhostScoresDidLoadHandler handler)
		{
			_handler = handler;
			OKLeaderboard leaderboard = new OKLeaderboard();
			leaderboard.LeaderboardID = this.LeaderboardID;

			// Kick off the chain...
			leaderboard.GetFacebookFriendsScores(FacebookFriendsScoresDidLoad);
		}

		private void ScoreDidLoadMetadata(OKScore score)
		{
			_pending.Remove(score);
			if(_pending.Count == 0)
				_handler(this);
		}

		private void FacebookFriendsScoresDidLoad(List<OKScore> scoresList, OKException e)
		{
			if(e == null) {
				_scores = scoresList;
				foreach (OKScore score in _scores) {
					if (score.MetadataBuffer == null && score.MetadataLocation != null) {
						_pending.Add(score);
						score.LoadMetadataBuffer(this.ScoreDidLoadMetadata);
					}
				}
				Debug.Log("Number of social scores: " + _scores.Count);
				Debug.Log("Number we need to pull metadata of: " + _pending.Count);
				if (_pending.Count == 0) {
					_handler(this);
				}
			} else {
				Debug.Log("Failed to get social scores: " + e.Message);
			}
		}
	}


	void GetSocialScores()
	{
		OKGhostScoreLoader loader = new OKGhostScoreLoader(SampleLeaderboardID);
		loader.ExecuteAsync((sender) => {

			// Do stuff with sender.Scores here.
			//
			// At this point, all scores in sender.Scores are guaranteed to
			// have the metadataBuffer loaded on them.

			foreach (OKScore score in sender.Scores) {
				UnityEngine.Debug.Log("Writing first five bytes of metadataBuffer for score: " + score.ScoreID);
				String s;
				for (int i = 0; i < 5; i++) {
					s = String.Format("Byte {0} - Hex: {1:X}", i, score.MetadataBuffer[i]);
					UnityEngine.Debug.Log("Got back: " + s);
				}
			}
		});
	}

	void GetMyBestScore()
	{
		OKLeaderboard leaderboard = new OKLeaderboard();
		leaderboard.LeaderboardID = SampleLeaderboardID;
		leaderboard.GetUsersTopScore((score, err) => {
			if (err == null) {
				if (score == null) {
					UnityEngine.Debug.Log("User does not have a score for this leaderboard.");
				} else {
					UnityEngine.Debug.Log("Got user's best score: " + score);
				}
			} else {
				UnityEngine.Debug.Log("Error getting best score: " + err.Message);
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
#if !UNITY_EDITOR
		GUI.matrix = GetScaleMatrix();
#endif
		Rect area = (IsPortraitOrientation() ? new Rect(0, 0, 320, 480) : new Rect(0, 0, 480, 320));
		GUILayout.BeginArea(area);
		GUILayoutOption h = GUILayout.Height(35);

		GUILayout.Label("Testing OpenKit...");

		if(GUILayout.Button("Show Leaderboards & Achievements", h)) {
			ShowLeaderboards();
		}

		if(GUILayout.Button("Show Leaderboards Landscape Only", h)) {
			OKManager.ShowLeaderboardsLandscapeOnly();
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


		if(GUILayout.Button("Logout from OpenKit", h)) {
			OKManager.LogoutCurrentUserFromOpenKit();
			OKLog.Info("logout of OpenKit");
		}

		if(GUILayout.Button("Get Leaderboards", h)) {
			OKLeaderboard.GetLeaderboards((List<OKLeaderboard> leaderboards, OKException exception) => {

				if(leaderboards != null){
					Debug.Log("Received " + leaderboards.Count + " leaderboards ");

					OKLeaderboard leaderboard = (OKLeaderboard)leaderboards[0];

					Debug.Log("Getting scores for leaderboard ID: " + leaderboard.LeaderboardID + " named: " + leaderboard.Name);
					leaderboard.GetGlobalScores(1,(List<OKScore> scores, OKException exception2) => {
						if(exception2 == null)
						{
							Debug.Log("Got global scores in the callback");
						}
					});
				} else {
					Debug.Log("Error getting leaderboards");
				}
			});
		}

		if(GUILayout.Button("Get social scores Friends", h)) {
			GetSocialScores();
		}

		if(GUILayout.Button("Get my best score!", h)) {
			GetMyBestScore();
		}

		GUILayout.EndArea();
	}
}
