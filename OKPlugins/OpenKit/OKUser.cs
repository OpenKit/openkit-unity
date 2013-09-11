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
			this.OKUserID = (int)userJSON.GetField("id").n;
			this.UserNick = userJSON.GetField("nick").str;
			this.FBUserID = userJSON.GetField("fb_id").str;
			this.CustomID = userJSON.GetField("custom_id").str;
			this.GoogleID = userJSON.GetField("google_id").str;
		}

		public int OKUserID {get; set;}
		public string FBUserID {get; set;}
		public string UserNick {get; set;}
		public string CustomID {get; set;}
		public string GoogleID {get; set;}

		public static OKUser GetCurrentUser()
		{
			return OKManager.GetCurrentUser();
		}
	}
}

