using System.Collections;
using OpenKit;

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
	
	class OKLeaderboard
	{
		public string Name { get; set; }
		public string IconUrl { get; set; }
		public int PlayerCount { get; set; }
		
		
		public OKLeaderboard() {}
		
		
	}
}
