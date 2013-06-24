/* ---------------------------------------------------------------------------------------------------- /
 *   The public API is in OKManager.cs.                                                                 *
 *   If you need to modify anything in this file please let me know at lzell11@gmail.com. Thanks!       *
 * ---------------------------------------------------------------------------------------------------- */
using System;
using UnityEngine;
using OpenKit;
using OpenKit.Native;
using System.Threading;

namespace OpenKit
{
	public class OKManagerImpl
	{
		private const string DEFAULT_ENDPOINT = "http://stage.openkit.io";
			
		// Synchronization
		private SynchronizationContext syncContext = null;
		private static IOKNativeBridge nativeBridge = null;
		
		#region Singleton Implementation
		// Utilizing singleton pattern (Not thread safe!  That should be ok).
		// http://msdn.microsoft.com/en-us/library/ff650316.aspx
		private static OKManagerImpl instance;
		public static OKManagerImpl Instance
		{
			get
			{
				if (instance == null) {
					instance = new OKManagerImpl();
				}
				return instance;
			}
		}
		
		private OKManagerImpl()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			nativeBridge = new OpenKitAndroid();
#elif UNITY_IPHONE && !UNITY_EDITOR
			nativeBridge = new OpenKitIOS();
#else
			nativeBridge = new OpenKitDummyObject();
#endif
			
			syncContext = SynchronizationContext.Current;
			if(syncContext == null)
				OKLog.Info("SynchronizationContext.Current is null.");
			else
				OKLog.Info("SynchronizationContext is set.");
			
			nativeBridge.setEndpoint(DEFAULT_ENDPOINT);
			endpoint = DEFAULT_ENDPOINT;
		}
		#endregion
		
		#region API
		private string appKey;
		public string AppKey 
		{
			get { return appKey; } 
			set
			{
				nativeBridge.setAppKey(value);
				appKey = value;
			}
		}
		
		private string endpoint;
		public string Endpoint 
		{ 
			get { return endpoint; }
			set	
			{ 
				nativeBridge.setEndpoint(value); 
				endpoint = value; 
			}
		}
		
		public void ShowLeaderboards()
		{
			nativeBridge.showLeaderboards();
		}
		
		public void ShowLoginToOpenKit()
		{
			nativeBridge.showLoginToOpenKit();
		}
		
		public OKUser GetCurrentUser()
		{
			return nativeBridge.getCurrentUser();
		}
				
		public void SubmitScore(OKScore score)
		{
			nativeBridge.submitScore(score);
		}
		
		public void SubmitAchievementScore(OKAchievementScore achievementScore)
		{
			nativeBridge.submitAchievementScore(achievementScore);
		}
		#endregion
		
		
		public void AuthenticateLocalPlayerWithGameCenter()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			OpenKitIOS openKit = (OpenKitIOS)nativeBridge;
			openKit.authenticateLocalPlayerToGC();
#else
			return;
#endif
		}
		
		#region Overrides
		// Called when logging object.
		public override string ToString()
		{
			return string.Format("{0}, Endpoint: {1}", base.ToString(), Endpoint);
		}
		#endregion
	}
}
