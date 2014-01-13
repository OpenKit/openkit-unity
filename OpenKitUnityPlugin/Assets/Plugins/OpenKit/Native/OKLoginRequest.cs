using System;
using UnityEngine;

namespace OpenKit
{
	public class OKLoginRequest : OKNativeAsyncCall
	{
		public override void CallNativeFunction(OKNativeAsyncCall dynamicObject)
		{
			OKManager.Instance._ShowLoginToOpenKit(dynamicObject);
		}
		
		public static void ShowLoginUIWithCallback(Action callback)
		{
			OKLog.Info("ShowLoginUI on OKLoginRequest called");
			GameObject gameObject = new GameObject("ShowOpenKitLoginUITempObject" + DateTime.Now.Ticks);
			OKLoginRequest loginRequest = gameObject.AddComponent<OKLoginRequest>();
			
			Action<bool,string> wrapperCallback = (success, stringRetVal) => {
				OKLog.Info("Wrapper callback called");
				callback();
			};
			loginRequest.callFunction(wrapperCallback);
		}
	}

}

