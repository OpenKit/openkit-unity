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

		public string Name { get; set; }
		public int OKLeaderboardID {get; set;}
		public LeaderboardSortType SortType {get; set;}
		public string IconUrl { get; set; }
		public int PlayerCount { get; set; }


		public OKLeaderboard() {}

		public OKLeaderboard(JSONObject leaderboardJSON)
		{
			this.Name = leaderboardJSON.GetField("name").str;
			this.OKLeaderboardID = (int)leaderboardJSON.GetField("id").n;
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
			OKCloudAsyncRequest.Get("/leaderboards", null, (JSONObject responseObj, OKCloudException e) => {
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
			requestParams.Add("leaderboard_id", this.OKLeaderboardID);
			requestParams.Add("page_num", pageNum);
			requestParams.Add("leaderboard_range","all_time");
			requestParams.Add("num_per_page",NUM_SCORES_PER_PAGE);

			this.GetScores("/best_scores", requestParams, requestHandler);
		}

		public void GetUsersTopScore(Action<OKScore,OKException> requestHandler)
		{
			this.GetUsersTopScore(OKUser.getCurrentUser().OKUserID, requestHandler);
		}

		private void GetUsersTopScore(int currentUserID, Action<OKScore,OKException> requestHandler)
		{
			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			requestParams.Add("leaderboard_id", this.OKLeaderboardID);
			requestParams.Add("leaderboard_range","all_time");
			requestParams.Add("user_id",currentUserID);

			OKCloudAsyncRequest.Get("/best_scores/user",requestParams, (JSONObject responseObj, OKCloudException e) => {
				if(e == null) {
					if(responseObj.type == JSONObject.Type.OBJECT) {
						Debug.Log("Succesfully got top score");
						OKScore topScore = new OKScore(responseObj);
						requestHandler(topScore, null);
					} else {
						requestHandler(null, new OKException("Expected a single score JSON object but got something else"));
					}
				} else
				{
					requestHandler(null, e);
				}
			});
		}

		// Helper function for getting scores from OpenKit, internal use only
		private void GetScores(string path, Dictionary<string, object> requestParams,Action<List<OKScore>, OKException> requestHandler)
		{
			OKCloudAsyncRequest.Get(path,requestParams, (JSONObject responseObj, OKCloudException e) => {
				if(e == null) {
					if(responseObj.type == JSONObject.Type.ARRAY) {
						Debug.Log("Succesfully got " + responseObj.list.Count + " scores");
						List<OKScore> scoresList = new List<OKScore>(responseObj.list.Count);

						for(int x = 0; x < responseObj.list.Count; x++)
						{
							OKScore score = new OKScore(responseObj[x]);
							scoresList.Add(score);
						}

						requestHandler(scoresList, null);
					} else {
						requestHandler(null, new OKException("Expected an array of scores but did not get back an Array JSON"));
					}
				} else
				{
					requestHandler(null, e);
				}
			});
		}

		public void GetFacebookFriendsScores(Action<List<OKScore>,OKException> requestHandler)
		{
			OKFacebookUtilities.getFacebookFriendsList((List<string> fbFriends,OKException e) => {
				if(e == null) {
					requestHandler(null, e);
				} else {
					this.GetFacebookFriendsScores(fbFriends, requestHandler);
				}
			});
		}

		private void GetFacebookFriendsScores(List<string> fbfriends, Action<List<OKScore>,OKException> requestHandler)
		{
			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			requestParams.Add("leaderboard_id", this.OKLeaderboardID);
			requestParams.Add("leaderboard_range","all_time");
			requestParams.Add("fb_friends",fbfriends);

			this.GetScores("/best_scores/social", requestParams, requestHandler);
		}


	}


}
