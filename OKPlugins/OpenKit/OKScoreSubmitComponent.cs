using System;
using UnityEngine;
namespace OpenKit
{
	public class OKScoreSubmitComponent : MonoBehaviour
	{		
		public OKScoreSubmitComponent(long scoreVal, int leaderboardID)
		{
			scoreValue = scoreVal;
			OKLeaderboardID = leaderboardID;
		}
		
		public long scoreValue {get; set;}
		public int OKLeaderboardID {get; set;}
		public int metadata {get; set;}
		public string displayString {get; set;}
		public string gameCenterLeaderboardCategory {get; set;}
		
		public Action<bool,string> submitScoreCallback;
		public string callbackGameObjectName;
		
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

