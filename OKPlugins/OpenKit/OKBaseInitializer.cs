using UnityEngine;
using OpenKit;

public class OKBaseInitializer : MonoBehaviour {

	void Awake()
	{
		gameObject.name = "OpenKitPrefab";
		DontDestroyOnLoad(gameObject);
	}

	// Forward native events to OKManager.  It will figure out what to do with them.
	private void NativeViewWillAppear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewWillAppear);
	}

	private void NativeViewDidAppear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewDidAppear);
	}

	private void NativeViewWillDisappear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewWillDisappear);
	}

	private void NativeViewDidDisappear(string empty)
	{
		OKManager.HandleNativeEvent(this, OKNativeEvent.viewDidDisappear);
	}

}
