using System.Collections;
using OpenKit;
using UnityEngine;

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
		
		public static void getLeaderboards()
		{	
			OKCloudAsyncRequest.Get("/leaderboards", null, (JSONObject responseObj, OKCloudException e) => {
				if(e != null) {
					OKLog.Error("Getting leaderboards failed with error: " + e);
				} else {
					Debug.Log("Got leaderboards, server response is: " + responseObj);
				}
				
			});
		}
	}
	
	
}
