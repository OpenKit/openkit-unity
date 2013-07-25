using UnityEngine;
using System.Collections;
using OpenKit;
using System;

namespace OpenKit
{
	// This class can't be abstract because it inherits from MonoBehavior 
	public class OKNativeAsyncCall : MonoBehaviour
	{
		public OKNativeAsyncCall ()
		{
		}
		
		private Action<bool,string> functionCallback;
		private string callbackGameObjectName;
		
		public void callFunction(Action<bool,string> callback)
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
			createdCallBackObject.callbackGameObjectName = callbackGameObjectName;
			
			callNativeFunction(createdCallBackObject);
			
#endif
		}
		
		// This method should be overridden
		public virtual void callNativeFunction(OKNativeAsyncCall dynamicGameObject) {
			Debug.Log("OKNativeAsyncCall callNativeFunction called instead of deriving class! Make sure you override callNativeFunction!");
		}
		
		public void asyncCallSucceeded(string paramString)
		{
			Debug.Log("asyncCallSucceeded");
			if(functionCallback != null) {
				functionCallback(true,paramString);
			}
			GameObject.Destroy(this.gameObject);
		}
		
		public void asyncCallFailed(string errorString)
		{
			
			Debug.Log("asyncCallFailed");
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

