using System;
using OpenKit;

namespace OpenKit
{
	public class OKUser
	{
		public OKUser ()
		{
		}

		public OKUser(JSONObject userJSON)
		{
			this.userNick = userJSON.GetField("nick").str;
			this.twitterUserID = (long)userJSON.GetField("twitter_id").n;
			this.OKUserID = (int)userJSON.GetField("id").n;
			this.FBUserID = (long)userJSON.GetField("fb_id").n;
			this.customID = (int)userJSON.GetField("custom_id").n;
		}

		public int OKUserID {get; set;}
		public long FBUserID {get; set;}
		public long twitterUserID {get; set;}
		public string userNick {get; set;}
		public int customID { get; set;}

		public static OKUser GetCurrentUser()
		{
			return OKManager.GetCurrentUser();
		}
	}
}

