using UnityEngine;
using System.Collections;
using OpenKit;
using System;

namespace OpenKit
{
	public abstract class OKBaseAsyncNativeFunctionCall : MonoBehaviour
	{
		public OKBaseAsyncNativeFunctionCall ()
		{
		}
		
		private Action<bool,string> functionCallback;
		private string callbackGameObjectName;
		
		public void submitScore(Action<bool,string> callback)
		{
			functionCallback = callback;
			string gameObjectName = "OpenKitBaseAsyncNativeFunctionCall."+DateTime.Now.Ticks;
			callbackGameObjectName = gameObjectName;
			
			
			// Create a new OKBaseAsyncNativeFunctionCall gameobject (called functionCall) and give it a unique name
			// This allows us to track unique calls to async native code
			
#if !UNITY_EDITOR	
			GameObject gameObject = new GameObject(gameObjectName);
			DontDestroyOnLoad(gameObject);
			
			OKBaseAsyncNativeFunctionCall createdCallBackObject = gameObject.AddComponent<OKBaseAsyncNativeFunctionCall>();
			createdCallBackObject.functionCallback = callback;
			
			callNativeFunction(createdCallBackObject);
			
#endif
		}
		
		public abstract void callNativeFunction(OKBaseAsyncNativeFunctionCall dynamicGameObject);
		
		public void asyncCallSucceeded(string paramString)
		{
			if(functionCallback != null) {
				functionCallback(true,paramString);
			}
			GameObject.Destroy(this.gameObject);
		}
		
		public void asyncCallFailed(string errorString)
		{
			if(functionCallback != null) {
				functionCallback(false, errorString);
			}
			
			GameObject.Destroy(this.gameObject);
		}
			
		public string GetCallbackGameObjectName()
		{
			return callbackGameObjectName;
		}
	}
}

