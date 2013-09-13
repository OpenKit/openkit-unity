using System;
using System.Collections;
using OpenKit;
using UnityEngine;
using System.Collections.Generic;


namespace OpenKit
{
	public enum LeaderboardSortType
	{
		HighValue,
		LowValue
	}

	public enum LeaderboardTimeRange
	{
		OneDay,
		OneWeek,
		AllTime
	}

	public class OKLeaderboard
	{
		private static int NUM_SCORES_PER_PAGE = 25;
		private static string DEFAULT_LEADERBOARD_LIST_TAG = "v1";
		private enum ScoreRequestType {
			Global,
			Social
		}

		public  string Name          { get; set; }
		public     int LeaderboardID { get; set; }
		public  string IconUrl       { get; set; }
		public     int PlayerCount   { get; set; }
		public LeaderboardSortType SortType { get; set; }


		public OKLeaderboard() {}

		public OKLeaderboard(JSONObject leaderboardJSON)
		{
			this.Name = leaderboardJSON.GetField("name").str;
			this.LeaderboardID = (int)leaderboardJSON.GetField("id").n;
			this.IconUrl = leaderboardJSON.GetField("icon_url").str;
			this.PlayerCount = (int)leaderboardJSON.GetField("player_count").n;

			string sortString = leaderboardJSON.GetField("sort_type").str;

			if(sortString.Equals("HighValue",System.StringComparison.CurrentCultureIgnoreCase)) {
				this.SortType = LeaderboardSortType.HighValue;
			} else {
				this.SortType = LeaderboardSortType.LowValue;
			}
		}
		
		public static void GetLeaderboards(Action<List<OKLeaderboard>, OKException> requestHandler)
		{
			// By default, if a leaderboard list tag is not defined through OKManger, we
		    // load the leaderboards with the tag = 'v1'. In the OK Dashboard, new leaderboards
		    // have a default tag of v1. This sets up future proofing so a developer can issue
		    // a set of leaderboards in the first version of their game, and then change the leaderboards
		    // in a future version of their game
			
			if(OKManager.Instance.GetLeaderboardListTag() == null) {
				GetLeaderboards(DEFAULT_LEADERBOARD_LIST_TAG, requestHandler);
			} else {
				GetLeaderboards(OKManager.Instance.GetLeaderboardListTag(), requestHandler);
			}
		}
		
		public static void GetLeaderboards(String leaderboardListTag, Action<List<OKLeaderboard>, OKException> requestHandler)
		{
			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			requestParams.Add("tag", leaderboardListTag);
			
			OKCloudAsyncRequest.Get("/leaderboards", requestParams, (JSONObject responseObj, OKCloudException e) => {
				if(e != null) {
					OKLog.Error("Getting leaderboards failed with error: " + e);
					requestHandler(null, e);

				} else {
					Debug.Log("Got leaderboards");
					if (responseObj.type == JSONObject.Type.ARRAY) {
						List<OKLeaderboard> leaderboardList = new List<OKLeaderboard>(responseObj.list.Count);

						for(int x = 0; x < responseObj.list.Count; x++)
						{
							OKLeaderboard leaderboard = new OKLeaderboard(responseObj[x]);
							leaderboardList.Add(leaderboard);
						}

						requestHandler(leaderboardList, null);
					} else {
						OKLog.Error("Expected an array of leaderboards but did not get back an Array JSON");
						requestHandler(null, new OKException("Expected an array of leaderboards but did not get back an Array JSON"));
					}
				}
			});
		}

		public void GetGlobalScores(int pageNum, Action<List<OKScore>, OKException> requestHandler)
		{
			if(pageNum <= 0)
				pageNum = 1;

			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			requestParams.Add("leaderboard_id", this.LeaderboardID);
			requestParams.Add("page_num", pageNum);
			requestParams.Add("leaderboard_range", "all_time");
			requestParams.Add("num_per_page", NUM_SCORES_PER_PAGE);
			GetScores(ScoreRequestType.Global, requestParams, requestHandler);
		}

		public void GetUsersTopScore(Action<OKScore,OKException> requestHandler)
		{	
			this.GetUsersTopScore(OKUser.GetCurrentUser(), requestHandler);
		}

		private void GetUsersTopScore(OKUser currentUser, Action<OKScore,OKException> requestHandler)
		{
			if(currentUser == null) {
				requestHandler(null, new OKException("No OKUser logged in, can't get best score"));
				return;
			}
			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			requestParams.Add("leaderboard_id", this.LeaderboardID);
			requestParams.Add("leaderboard_range","all_time");
			requestParams.Add("user_id",currentUser.OKUserID);
			
			OKLog.Info("About to make network request");
			OKCloudAsyncRequest.Get("/best_scores/user",requestParams, (JSONObject responseObj, OKCloudException e) => {
				if(e == null) {
					if(responseObj.type == JSONObject.Type.OBJECT) {
						OKScore topScore = new OKScore(responseObj);
						requestHandler(topScore, null);
					} else if (responseObj.type == JSONObject.Type.NULL) {
						requestHandler(null, null);
					} else {
						requestHandler(null, new OKException("Expected a single score JSON object but got something else"));
					}
				} else {
					requestHandler(null, e);
				}
			});
		}

		// Helper function for getting scores from OpenKit, internal use only
		private void GetScores(ScoreRequestType rt, Dictionary<string, object> requestParams,Action<List<OKScore>, OKException> requestHandler)
		{
			Action<JSONObject, OKCloudException> internalHandler = (responseObj, e) => {
				if(e == null) {
					if(responseObj.type == JSONObject.Type.ARRAY) {
						Debug.Log("Successfully got " + responseObj.list.Count + " scores");
						Debug.Log("Respones json: " + responseObj.ToString());
						List<OKScore> scoresList = new List<OKScore>(responseObj.list.Count);

						for(int x = 0; x < responseObj.list.Count; x++) {
							OKScore score = new OKScore(responseObj[x]);
							scoresList.Add(score);
						}

						requestHandler(scoresList, null);
					} else {
						requestHandler(null, new OKException("Expected an array of scores but did not get back an Array JSON"));
					}
				} else {
					requestHandler(null, e);
				}
			};

			switch (rt) {
				case ScoreRequestType.Global:
					OKCloudAsyncRequest.Get("/best_scores", requestParams, internalHandler);
					break;
				case ScoreRequestType.Social:
					OKCloudAsyncRequest.Post("/best_scores/social", requestParams, internalHandler);
					break;
			}
		}

		public void GetFacebookFriendsScores(Action<List<OKScore>,OKException> requestHandler)
		{
			OKFacebookUtilities.GetFacebookFriendsList((List<string> fbFriends,OKException e) => {
				if(e == null) {
					Debug.Log("Got facebook friends list");
					this.GetFacebookFriendsScores(fbFriends, requestHandler);
				} else {
					Debug.Log("Error getting list of fb friends");
					requestHandler(null, e);
				}
			});
		}

		private void GetFacebookFriendsScores(List<string> fbfriends, Action<List<OKScore>,OKException> requestHandler)
		{
			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			requestParams.Add("leaderboard_id", this.LeaderboardID);
			requestParams.Add("leaderboard_range", "all_time");
			requestParams.Add("fb_friends", fbfriends);
			GetScores(ScoreRequestType.Social, requestParams, requestHandler);
		}
	}
}
