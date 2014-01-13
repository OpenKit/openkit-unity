package com.facebook.unity;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.facebook.Session;
import com.unity3d.player.UnityPlayerNativeActivity;

public class FBUnityPlayerNativeActivity extends UnityPlayerNativeActivity {
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
	  super.onActivityResult(requestCode, resultCode, data);
	  Session.getActiveSession().onActivityResult(this, requestCode, resultCode, data);
	}

    @Override
    public void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        FB.SetIntent(intent);
    }
}
