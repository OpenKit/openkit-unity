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
		
		public static void getLeaderboards(Action<IList, OKException> requestHandler)
		{	
			OKCloudAsyncRequest.Get("/leaderboards", null, (JSONObject responseObj, OKCloudException e) => {
				if(e != null) {
					OKLog.Error("Getting leaderboards failed with error: " + e);
					requestHandler(null, e);
					
				} else {
					//Debug.Log("Got leaderboards, server response is: " + responseObj);
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
		
		
		
		
	}
	
	
}
