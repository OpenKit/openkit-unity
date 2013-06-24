using UnityEngine;
using System.Collections;
using OpenKit;
using System;

namespace OpenKit
{
	public class OKScore : MonoBehaviour
	{
		public OKScore (long scoreVal, int leaderboardID)
		{
			scoreValue = scoreVal;
			OKLeaderboardID = leaderboardID;
		}
		
		public long scoreValue {get; set;}
		public int OKLeaderboardID {get; set;}
		public int metadata {get; set;}
		public string displayString {get; set;}
		public string gameCenterLeaderboardCategory {get; set;}
		
		
		//Not used for now
		//public OKUser user {get; set;}
		//public int rank {get; set;}
		//public int OKScoreID {get; set;}
		
		private Action<bool,string> submitScoreCallback;
		private string callbackGameObjectName;
		
		public void submitScore(Action<bool,string> callback)
		{
			submitScoreCallback = callback;
			string gameObjectName = "OpenKitSubmitScoreObject."+DateTime.Now.Ticks;
			callbackGameObjectName = gameObjectName;
			
			
			// Create a new OKScore gameobject (called scoreComponent) and give it a unique name
			// This allows us to track unique score submission requests and handle
			// async native code
			
#if !UNITY_EDITOR	
			GameObject gameObject = new GameObject(gameObjectName);
			DontDestroyOnLoad(gameObject);
			
			OKScore scoreComponent = gameObject.AddComponent<OKScore>();
			scoreComponent.submitScoreCallback = callback;
			
			scoreComponent.scoreValue = scoreValue;
			scoreComponent.OKLeaderboardID = OKLeaderboardID;
			scoreComponent.displayString = displayString;
			scoreComponent.metadata = metadata;
			scoreComponent.callbackGameObjectName = gameObjectName;
			scoreComponent.gameCenterLeaderboardCategory = gameCenterLeaderboardCategory;
			
			OKManager.SubmitScore(scoreComponent);
#endif
		}
		
		public void scoreSubmissionSucceeded()
		{
			if(submitScoreCallback != null)
			{
				submitScoreCallback(true,"");
			}
			GameObject.Destroy(this.gameObject);
		}
		
		public void scoreSubmissionFailed(string errorString)
		{
			if(submitScoreCallback != null) {
				submitScoreCallback(false, errorString);
			}
			
			GameObject.Destroy(this.gameObject);
		}
		
		public string GetCallbackGameObjectName()
		{
			return callbackGameObjectName;
		}
	}
}

