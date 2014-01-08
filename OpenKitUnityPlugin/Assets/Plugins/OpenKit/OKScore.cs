using System;
using System.Collections.Generic;
using UnityEngine;
using RestSharp;

namespace OpenKit
{
	public class OKScore
	{
		public static OKUser GetUser()
		{
			return OKManager.GetCurrentUser();
		}

		public OKScore(long scoreVal, int leaderboardID)
		{
			scoreValue = scoreVal;
			LeaderboardID = leaderboardID;
		}

		// Use this in case a field doesn't come back in the JSON.
		public object SafeMap<T>(JSONObject json, string field, object defaultTo)
		{
			JSONObject x = json.GetField(field);
			if (x == null)
				return defaultTo;

			if (typeof(T) == typeof(int)) {
				return (int)x.n;
			} else if (typeof(T) == typeof(long)) {
				return (long)x.n;
			} else if (typeof(T) == typeof(string)) {
				return x.str;
			}

			throw new Exception("OK: Unknown T passed to SafeMap");
		}

		public OKScore(JSONObject scoreJSON)
		{
			this.LeaderboardID   = (int)SafeMap<int>(scoreJSON, "leaderboard_id", -1);
			this.ScoreID         = (int)SafeMap<int>(scoreJSON, "id", -1);
			this.scoreValue      = (long)SafeMap<long>(scoreJSON, "value", 0);
			this.displayString   = (string)SafeMap<string>(scoreJSON, "display_string", null);
			this.metadata        = (int)SafeMap<int>(scoreJSON, "metadata", 0);
			this.scoreRank       = (int)SafeMap<int>(scoreJSON, "rank", 0);
			this.MetadataLocation  = (string)SafeMap<string>(scoreJSON, "meta_doc_url", null);

			JSONObject u = scoreJSON.GetField("user");
			if (u != null)
				this.user = new OKUser(u);
		}

		public    long scoreValue                    { get; set; }
		public     int LeaderboardID                 { get; set; }
		public     int metadata                      { get; set; }
		public  string displayString                 { get; set; }
		public  string gameCenterLeaderboardCategory { get; set; }
		public     int ScoreID                       { get; protected set; }
		public     int scoreRank                     { get; set; }
		public  OKUser user                          { get; protected set; }
		public  string MetadataLocation              { get; set; }
		public  byte[] MetadataBuffer                { get; set; }

		private OKMetadataRequest _metadataRequest = null;

		public void SubmitScoreNatively(Action<bool,string> callback)
		{
			if (MetadataBuffer != null) {
				UnityEngine.Debug.LogError("A score with MetadataBuffer set cannot be submitted via native plugin");
				callback(false, null);
			}

			// Create a uniquely named GameObject, and keep it around between scene switches.
			string gameObjectName = "OpenKitSubmitScoreObject." + DateTime.Now.Ticks;
			GameObject gameObject = new GameObject(gameObjectName);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);

			OKScoreSubmitComponent scoreSubmitComponent = gameObject.AddComponent<OKScoreSubmitComponent>();
			scoreSubmitComponent.submitScoreCallback           = callback;
			scoreSubmitComponent.callbackGameObjectName        = gameObjectName;
			scoreSubmitComponent.scoreValue                    = scoreValue;
			scoreSubmitComponent.OKLeaderboardID               = LeaderboardID;
			scoreSubmitComponent.displayString                 = displayString;
			scoreSubmitComponent.metadata                      = metadata;
			scoreSubmitComponent.gameCenterLeaderboardCategory = gameCenterLeaderboardCategory;

			OKManager.SubmitScore(scoreSubmitComponent);
		}

		// Wrapper method to keep previous API working
		public void SubmitScore(Action<bool,string> callback)
		{
			SubmitScoreNatively(callback);
		}

		public void SubmitScore(Action<OKScore, OKException> callback)
		{
			OKUser u = GetUser();
			if (u == null)
				throw new Exception("You need a user to submit a score");

			Dictionary<string, object> score = new Dictionary<string, object>();
			score.Add("leaderboard_id", LeaderboardID);
			score.Add("value"         , scoreValue);
			score.Add("display_string", displayString);
			score.Add("metadata"      , metadata);
			score.Add("user_id"       , u.OKUserID.ToString());

			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("score", score);

			OKUploadBuffer buff = null;
			if (MetadataBuffer != null) {
				buff = new OKUploadBuffer() {
					Bytes = MetadataBuffer,
					ParamName = "score[meta_doc]",
					FileName = "upload"
				};
			}

			Action<JSONObject, OKCloudException>handler = (responseObj, e) => {
				if(e == null) {
					OKScore retScore = new OKScore(responseObj);
					callback(retScore, null);
				} else {
					callback(null, e);
				}
			};

			OKCloudAsyncRequest.Post("/scores", reqParams, buff, handler);
		}

		public void submitScoreOnlyToOpenKit(Action<bool,string> callback)
		{
		}

		public void LoadMetadataBuffer(Action<OKScore> handler)
		{
			if (MetadataLocation != null) {
				_metadataRequest = OKMetadataRequest.Get(MetadataLocation, res => {
					if (res.Status == OKIOStatus.Succeeded) {
						MetadataBuffer = res.Raw;
					}
					handler(this);
					_metadataRequest = null;
				});
			}
		}

		public bool MetadataRequestInProgress()
		{
			return (_metadataRequest != null);
		}

		public void CancelMetadataRequest()
		{
			if (MetadataRequestInProgress()) {
				OKLog.Info("Cancelling the request for metadata of score: " + ScoreID);
				_metadataRequest.Cancel();
			}
		}

		public override string ToString()
		{
			return string.Format("OKScore(id: {0}, leaderboard_id: {1}, display_string: {2})", ScoreID, LeaderboardID, displayString);
		}
	}
}

