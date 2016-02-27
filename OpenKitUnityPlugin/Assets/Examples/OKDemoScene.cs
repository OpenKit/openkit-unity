using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenKit;
using System;
using Facebook.Unity;

public class OKDemoScene : MonoBehaviour {

	private const int SampleLeaderboardID = 385;
	private const String SampleLeaderboardGameCenterCategory = "level1";
	private bool submitScoreNatively = true;
	
	
	void Start()
	{
		Setup();
	}
	
	void Setup()
	{
		// Authenticate the local player with GameCenter (iOS only).
		OKManager.authenticateGameCenterLocalPlayer();

		// Listen for native openkit view events.
		OKManager.ViewWillAppear    += ViewWillAppear;
		OKManager.ViewDidAppear     += ViewDidAppear;
		OKManager.ViewWillDisappear += ViewWillDisappear;
		OKManager.ViewDidDisappear  += ViewDidDisappear;

		if(OKManager.IsCurrentUserAuthenticated()) {
			OKLog.Info("Found OpenKit user");
		} else {
			ShowLoginUI();
			OKLog.Info("Did not find OpenKit user");
		}
	}

	static void ViewWillAppear(object sender, EventArgs e) {
		OKLog.Info("OK ViewWillAppear");
	}

	static void ViewWillDisappear(object sender, EventArgs e) {
		OKLog.Info("OK ViewWillDisappear");
	}

	static void ViewDidAppear(object sender, EventArgs e) {
		OKLog.Info("OK ViewDidAppear");
	}

	static void ViewDidDisappear(object sender, EventArgs e) {
		OKLog.Info("OK ViewDidDisappear");
	}


	void ShowLeaderboards()
	{
		OKManager.ShowLeaderboards();
	}

	void ShowAchievements()
	{
		OKManager.ShowAchievements();
	}
	void ShowLoginUI()
	{
		OKLog.Info("Showing login UI");
		OKManager.ShowLoginToOpenKitWithDismissCallback(() => {
			OKLog.Info("Finished showing OpenKit login window, in the callback");
		});

		OKLog.Info("Is fb session Open: " + OKManager.IsFBSessionOpen());
	}


	// Notes about posting a score:
	//
	// If the user is not logged in, the score will not be submitted successfully.
	//
	// When submitting a score natively, if the score submission fails, the score is cached locally on the device and resubmitted
	// when the user logs in or next time the app loads, whichever comes first.
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
		int lapTime = 5400;  // value in hundredths
		int total_sec = lapTime / 100;
		int total_min = total_sec / 60;
		int hour = total_min / 60;
		int min = total_min % 60;
		int sec = total_sec % 60;
		int hun = lapTime % 100;

		string scoreString = "" + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + "." + hun.ToString("00");

		OKScore score = new OKScore(lapTime, SampleLeaderboardID);
		score.displayString = scoreString;
		score.gameCenterLeaderboardCategory = SampleLeaderboardGameCenterCategory;
		
		// OKScore can be submitted to OpenKit in C# native unity, or platform native code (e.g. iOS and Android native cdoe).
		// When possible you should use the platform native versions of OKScore.SubmitScore because both iOS and Android SDKs
		// have local caching built in, as well as features like submit to GameCenter (iOS).

		Action<bool, string> nativeHandle = (success, errorMessage) => {
			if (success) {
				OKLog.Info("Score submitted successfully!");
			} else {
				OKLog.Info("Score did not submit. Error: " + errorMessage);
			}
		};

		Action<OKScore, OKException> defaultHandle = (retScore, err) => {
			if (err == null) {
				OKLog.Info("Score submitted successfully: " + retScore.ToString());
			} else {
				OKLog.Info("Score post failed: " + err.Message);
			}
		};

		if(submitScoreNatively) {
			score.SubmitScoreNatively(nativeHandle);
		}
		else {
			score.MetadataBuffer = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x80 };
			score.SubmitScore(defaultHandle);
		}
	}


	void UnlockSampleAchievement()
	{
		int SampleAchievementID = 189;
		int SampleAchievementProgress = 10;
		string SampleAchievementGamecenterID = "achievement2";

		OKAchievementScore achievementScore = new OKAchievementScore(SampleAchievementProgress, SampleAchievementID);

		// On iOS, we can also support GameCenter achievements with this simple wrapper
		achievementScore.GameCenterAchievementIdentifier = SampleAchievementGamecenterID;
		achievementScore.GameCenterAchievementPercentComplete = 100.0f;

		achievementScore.submitAchievementScore((success, errorMessage) => {
			if (success) {
				OKLog.Info("Achievement score/progress submitted successfully!");
			} else {
				OKLog.Info("Achievement score/progress did not submit. Error: " + errorMessage);
			}
		});
	}

	void UnlockSampleGamecenterAchievementOnly()
	{
		OKAchievementScore score = new OKAchievementScore();
		score.GameCenterAchievementIdentifier = "achievement2";
		score.GameCenterAchievementPercentComplete = 100f;
		score.submitAchievementScore((success, errorMessage) => {
			if (success) {
				OKLog.Info("GC achievement score/progress submitted successfully!");
			} else {
				OKLog.Info("GC achievement score/progress did not submit. Error: " + errorMessage);
			}
		});
	}

	// Get the list of leaderboards in C# (native unity)
	void GetLeaderboards()
	{
		OKLeaderboard.GetLeaderboards((List<OKLeaderboard> leaderboards, OKException exception) => {

				if(leaderboards != null){
					OKLog.Info("Received " + leaderboards.Count + " leaderboards ");

					OKLeaderboard leaderboard = (OKLeaderboard)leaderboards[0];

					OKLog.Info("Getting scores for leaderboard ID: " + leaderboard.LeaderboardID + " named: " + leaderboard.Name);
					leaderboard.GetGlobalScores(1,(List<OKScore> scores, OKException exception2) => {
						if(exception2 == null)
						{
							OKLog.Info("Got global scores in the callback");
						}
					});
				} else {
					OKLog.Info("Error getting leaderboards");
				}
			});
	}
	
		void GetSocialScores()
		{
	
			OKLeaderboard leaderboard = new OKLeaderboard(SampleLeaderboardID);

			OKLog.Info("Getting scores for leaderboard ID: " + leaderboard.LeaderboardID + " named: " + leaderboard.Name);
			leaderboard.GetFacebookFriendsScores((List<OKScore> scores, OKException exception2) => {
				if(exception2 == null) {
					OKLog.Info("Got facebook friends scores scores in the callback");
				} else {
					OKLog.Info("Error getting facebook friends scores: " + exception2);
				}
			});
					
		}


	public void CancelGhostRequest(object state)
	{
		GhostScoresRequest request = (GhostScoresRequest)state;
		request.Cancel();
	}

	void GetScoresWithMetadata()
	{
		var leaderboard = new OKLeaderboard(SampleLeaderboardID);
		var request = new GhostScoresRequest(leaderboard);

		request.Get(response => {
			switch (response.Status) {
				case OKIOStatus.Cancelled:
					OKLog.Info("Cancelled the ghost scores request.");
					break;
				case OKIOStatus.FailedWithError:
					OKLog.Info("Ghost scores request failed with error: " + response.Err.Message);
					break;
				case OKIOStatus.Succeeded:
					OKLog.Info("Ghost ghost scores!");
					WriteMetadata(response.scores);
					break;
			}
		});

		// Cancel the request anytime with:
		// request.Cancel();

		// new System.Threading.Timer(CancelGhostRequest, request, 150, -1);
	}

	private void WriteMetadata(List<OKScore> scores)
	{
		foreach (OKScore score in scores) {
			if (score.MetadataBuffer == null) {
				OKLog.Info("Score does not have a metadata buffer: " +  score.ScoreID);
				continue;
			}
			OKLog.Info("Writing first five bytes of metadataBuffer for score: " + score.ScoreID);
			String s;
			for (int i = 0; i < 5; i++) {
				s = String.Format("Byte {0} - Hex: {1:X}", i, score.MetadataBuffer[i]);
				OKLog.Info("Got back: " + s);
			}
		}
	}

	void GetMyBestScore()
	{
		OKLeaderboard leaderboard = new OKLeaderboard();
		leaderboard.LeaderboardID = SampleLeaderboardID;
		leaderboard.GetUsersTopScore((score, err) => {
			if (err == null) {
				if (score == null) {
					OKLog.Info("User does not have a score for this leaderboard.");
				} else {
					OKLog.Info("Got user's best score: " + score);
				}
			} else {
				OKLog.Info("Error getting best score: " + err.Message);
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

		GUILayout.Label("OpenKit Demo Scene");

		if(GUILayout.Button("Show Leaderboards", h)) {
			ShowLeaderboards();
		}

		if(GUILayout.Button("Show Achievements", h)) {
			ShowAchievements();
		}

		if(GUILayout.Button("Show Single Leaderboard", h)) {
			// Instead of showing a list of leaderboards, show a single specified leaderboard ID
			OKManager.ShowLeaderboard(SampleLeaderboardID);;
		}

		if(GUILayout.Button("Show Leaderboards & Achievements", h)) {
			OKManager.ShowLeaderboardsAndAchivements();
		}

		if(GUILayout.Button("Show Login UI", h)) {
			ShowLoginUI();
		}

		if(GUILayout.Button("Submit Score to Level 1 Leaderboard", h)) {
			SubmitSampleScore();
		}

		if(GUILayout.Button("Unlock Achievement", h)) {
			UnlockSampleAchievement();
		}

		if(GUILayout.Button("Logout from OpenKit", h)) {
			OKManager.LogoutCurrentUserFromOpenKit();
			OKLog.Info("logout of OpenKit");
		}

		if(GUILayout.Button("Get Leaderboards and global scores in C#", h)) {
			GetLeaderboards();
		}
		
		if(GUILayout.Button("Get my best score (in C#)", h)) {
			GetMyBestScore();
		}
		
		if(GUILayout.Button("Get friends scores in C#",h)) {
			GetSocialScores();
		}
		/*
		if(GUILayout.Button("FB SDK Test",h)) {

			if(FB.IsLoggedIn) {
				GetFBInfo();
				return;
			}

			FB.Init(() => { 
				OKLog.Info("FB Init called");

				if(FB.IsLoggedIn) {
					OKLog.Info("logged into FB in Unity");
					GetFBInfo();
				} else {
					OKLog.Info("not logged into FB unity");
					FB.Login("email",(FBResult result) => {
						OKLog.Info("Result of calling fb login: " + result.Text);
					});
				}

			});
		}
		*/

		if(GUILayout.Button("Get scores with metadata", h)) {
			GetScoresWithMetadata();
		}

		GUILayout.EndArea();
	}

	void GetFBInfo()
	{
		FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", HttpMethod.GET, (IGraphResult result) => {
			OKLog.Info ("Unity Result from FB is: " + result.RawResult);
		}); 

	}
}
