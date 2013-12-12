using UnityEngine;
using System.Collections;
using OpenKit;
using System;

namespace OpenKit
{
	public class OKAchievementScore : MonoBehaviour
	{
		public OKAchievementScore (int achievementProgress, int achievementID)
		{
			progress = achievementProgress;
			OKAchievementID = achievementID;
		}

		public int progress {get; set;}
		public int OKAchievementID {get; set;}
		public string GameCenterAchievementIdentifier {get; set;}
		public float GameCenterAchievementProgress {get; set;}

		private Action<bool,string> submitAchievementScoreCallback;
		private string callbackGameObjectName;

		public void submitAchievementScore(Action<bool,string> callback)
		{
			submitAchievementScoreCallback = callback;
			string gameObjectName = "OpenKitSubmitAchievementScoreObject."+DateTime.Now.Ticks;
			callbackGameObjectName = gameObjectName;


			// Create a new OKAchievementScore gameobject (called scoreComponent) and give it a unique name
			// This allows us to track unique score submission requests and handle
			// async native code

#if !UNITY_EDITOR
			GameObject gameObject = new GameObject(gameObjectName);
			DontDestroyOnLoad(gameObject);

			OKAchievementScore achievementScoreComponent = gameObject.AddComponent<OKAchievementScore>();
			achievementScoreComponent.submitAchievementScoreCallback = callback;

			achievementScoreComponent.progress = progress;
			achievementScoreComponent.OKAchievementID = OKAchievementID;
			achievementScoreComponent.callbackGameObjectName = gameObjectName;

			OKManager.SubmitAchievementScore(achievementScoreComponent);
#endif
		}

		public void scoreSubmissionSucceeded()
		{
			if(submitAchievementScoreCallback != null)
			{
				submitAchievementScoreCallback(true,"");
			}
			GameObject.Destroy(this.gameObject);
		}

		public void scoreSubmissionFailed(string errorString)
		{
			if(submitAchievementScoreCallback != null) {
				submitAchievementScoreCallback(false, errorString);
			}

			GameObject.Destroy(this.gameObject);
		}

		public string GetCallbackGameObjectName()
		{
			return callbackGameObjectName;
		}
	}
}

