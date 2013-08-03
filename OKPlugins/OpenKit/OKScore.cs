using System;
using System.Collections.Generic;
using UnityEngine;

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
			OKLeaderboardID = leaderboardID;
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
			this.OKLeaderboardID = (int)SafeMap<int>(scoreJSON, "leaderboard_id", -1);
			this.OKScoreID       = (int)SafeMap<int>(scoreJSON, "id", -1);
			this.scoreValue      = (long)SafeMap<long>(scoreJSON, "value", 0);
			this.displayString   = (string)SafeMap<string>(scoreJSON, "display_string", null);
			this.metadata        = (int)SafeMap<int>(scoreJSON, "metadata", 0);
			this.scoreRank       = (int)SafeMap<int>(scoreJSON, "rank", 0);

			JSONObject u = scoreJSON.GetField("user");
			if (u != null)
				this.user = new OKUser(u);
		}

		public    long scoreValue                    { get; set; }
		public     int OKLeaderboardID               { get; set; }
		public     int metadata                      { get; set; }
		public  string displayString                 { get; set; }
		public  string gameCenterLeaderboardCategory { get; set; }
		public     int OKScoreID                     { get; protected set; }
		public     int scoreRank                     { get; set; }
		public  OKUser user                          { get; protected set; }

		public  string Filename                      { get; set; }


		public void SubmitScore(Action<OKScore, OKException> callback)
		{
			OKUser u = GetUser();
			if (u == null)
				throw new Exception("You need a user to perform cloud set.");

			Dictionary<string, object> score = new Dictionary<string, object>();
			score.Add("leaderboard_id", OKLeaderboardID);
			score.Add("value"         , scoreValue);
			score.Add("display_string", displayString);
			score.Add("metadata"      , metadata);
			score.Add("user_id"       , u.OKUserID.ToString());

			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("score", score);

			OKCloudAsyncRequest.Post("/scores", reqParams, this.Filename, (JSONObject responseObj, OKCloudException e) => {
				if(e == null) {
					OKScore retScore = new OKScore(responseObj);
					callback(retScore, null);
				} else {
					OKException retErr = new OKException("Failed to create score " + e);
					callback(null, retErr);
				}
			});
		}

		public void submitScoreOnlyToOpenKit(Action<bool,string> callback)
		{
		}

		public override string ToString()
		{
			return string.Format("OKScore(id: {0}, leaderboard_id: {1}, display_string: {2})", OKScoreID, OKLeaderboardID, displayString);
		}
	}
}

