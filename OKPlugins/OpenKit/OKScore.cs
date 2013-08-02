using System;
using UnityEngine;

namespace OpenKit
{
	public class OKScore
	{
		public OKScore(long scoreVal, int leaderboardID)
		{
			scoreValue = scoreVal;
			OKLeaderboardID = leaderboardID;
		}

		public OKScore(JSONObject scoreJSON)
		{
			this.OKLeaderboardID = (int)scoreJSON.GetField("leaderboard_id").n;
			this.OKScoreID = (int)scoreJSON.GetField("id").n;
			this.scoreValue = (long)scoreJSON.GetField("value").n;
			this.scoreRank = (int)scoreJSON.GetField("rank").n;
			this.displayString = scoreJSON.GetField("display_string").str;
			this.metadata = (int)scoreJSON.GetField("metadata").n;
			this.user = new OKUser(scoreJSON.GetField("user"));
		}

		public long scoreValue {get; set;}
		public int OKLeaderboardID {get; set;}
		public int metadata {get; set;}
		public string displayString {get; set;}
		public string gameCenterLeaderboardCategory {get; set;}
		public int OKScoreID {get; protected set;}
		public int scoreRank {get; set;}
		public OKUser user {get; protected set;}

		public void submitScore(Action<bool,string> callback)
		{
			//Create a unique name for a new game object
			string gameObjectName = "OpenKitSubmitScoreObject."+DateTime.Now.Ticks;

			GameObject gameObject = new GameObject(gameObjectName);
			//Call don't destroy on load so that level changes don't destroy this gameobject
			UnityEngine.Object.DontDestroyOnLoad(gameObject);

			//Add an instance of a scoreComponent to the newly created gameobject, set all the score properties
			OKScoreSubmitComponent scoreSubmitComponent = gameObject.AddComponent<OKScoreSubmitComponent>();
			scoreSubmitComponent.submitScoreCallback = callback;
			scoreSubmitComponent.callbackGameObjectName = gameObjectName;

			scoreSubmitComponent.scoreValue = scoreValue;
			scoreSubmitComponent.OKLeaderboardID = OKLeaderboardID;
			scoreSubmitComponent.displayString = displayString;
			scoreSubmitComponent.metadata = metadata;
			scoreSubmitComponent.gameCenterLeaderboardCategory = gameCenterLeaderboardCategory;

			OKManagerImpl.Instance.SubmitScore(scoreSubmitComponent);
		}

		public void submitScoreOnlyToOpenKit(Action<bool,string> callback)
		{

		}
	}
}

