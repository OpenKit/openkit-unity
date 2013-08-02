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

			// This allows us to track unique calls to async native code

#if !UNITY_EDITOR
			GameObject gameObject = new GameObject(gameObjectName);
			DontDestroyOnLoad(gameObject);

			OKNativeAsyncCall createdCallBackObject = gameObject.AddComponent<OKNativeAsyncCall>();
			createdCallBackObject.functionCallback = callback;
			createdCallBackObject.callbackGameObjectName = callbackGameObjectName;

			CallNativeFunction(createdCallBackObject);
#else
			asyncCallFailed("OpenKit native calls are not supported in the Unity editor");
#endif


		}

		// This method should be overridden
		public virtual void CallNativeFunction(OKNativeAsyncCall dynamicGameObject) {
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

			Debug.Log("asyncCallFailed: " + errorString);
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

