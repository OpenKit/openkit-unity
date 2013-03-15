using System;
using OpenKit;

namespace OpenKit
{
	public class OKUser
	{
		public OKUser ()
		{
		}
		
		public int OKUserID {get; set;}
		public long FBUserID {get; set;}
		public long twitterUserID {get; set;}
		public string userNick {get; set;}
		
		public static OKUser getCurrentUser()
		{
			return OKManager.GetCurrentUser();
		}
	}
}

