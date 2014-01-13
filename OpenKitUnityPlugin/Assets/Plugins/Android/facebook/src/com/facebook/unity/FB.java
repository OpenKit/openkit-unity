package com.facebook.unity;

import java.io.Serializable;
import java.math.BigDecimal;
import java.util.*;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

import android.content.Intent;
import android.net.Uri;
import android.text.TextUtils;
import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.util.Base64;
import android.content.pm.*;
import android.content.pm.PackageManager.NameNotFoundException;

import com.facebook.*;
import com.facebook.Session.Builder;
import com.facebook.Session.OpenRequest;
import com.facebook.Session.StatusCallback;
import com.facebook.model.*;
import com.facebook.widget.WebDialog;
import com.facebook.widget.WebDialog.OnCompleteListener;
import com.unity3d.player.UnityPlayer;

public class FB {
	private static final String TAG = "FBUnitySDK";
	// i.e. the game object that receives this message
	private static final String FB_UNITY_OBJECT = "UnityFacebookSDKPlugin";
	private static Session session;
    private static Intent intent;
    private static AppEventsLogger appEventsLogger;

	// if we have a session it has been opened.
	private static void setSession(Session session) {
		FB.session = session;
	}

    private static AppEventsLogger getAppEventsLogger() {
        if (appEventsLogger == null) {
            appEventsLogger = AppEventsLogger.newLogger(getActivity().getApplicationContext());
        }
        return appEventsLogger;
    }

	public static class UnityMessage {
		private String methodName;
		private Map<String, Serializable> params = new HashMap<String, Serializable>();

		public UnityMessage(String methodName) {
			this.methodName = methodName;
		}

		public UnityMessage put(String name, Serializable value) {
			params.put(name, value);
			return this;
		}

		public UnityMessage putCancelled() {
			put("cancelled", true);
			return this;
		}

		public UnityMessage putID(String id) {
			put("id", id);
			return this;
		}

		public void sendNotLoggedInError() {
			sendError("not logged in");
		}

		public void sendError(String errorMsg) {
			this.put("error", errorMsg);
			send();
		}

		public void send() {
			assert methodName != null : "no method specified";
			String message = new JSONObject(this.params).toString();
			Log.v(TAG,"sending to Unity "+this.methodName+"("+message+")");
			UnityPlayer.UnitySendMessage(FB_UNITY_OBJECT, this.methodName, message);
		}
	}

	private static boolean isLoggedIn() {
		return Session.getActiveSession() != null && Session.getActiveSession().isOpened();
	}

	private static Activity getActivity() {
		return UnityPlayer.currentActivity;
	}

	private static void initAndLogin(String params, final boolean show_login_dialog) {

        Session session = (FB.isLoggedIn()) ? Session.getActiveSession() : new Builder(getActivity()).build();
        final UnityMessage unityMessage = new UnityMessage((show_login_dialog) ? "OnLoginComplete" : "OnInitComplete");

        // add the key hash to the JSON dictionary
        // unityMessage.put("key_hash", "test_key_and");
        unityMessage.put("key_hash", getKeyHash());

        // if we have a session and are init-ing, we can just return here.
        if (!SessionState.CREATED_TOKEN_LOADED.equals(session.getState()) && !show_login_dialog) {
            unityMessage.send();
            return;
        }

		// parse and separate the permissions into read and publish permissions
        String[] parts = null;
        final JSONObject unity_params;
        try {
            unity_params = new JSONObject(params);

            if (!unity_params.isNull("scope") && !unity_params.getString("scope").equals("")) {
                parts = unity_params.getString("scope").split(",");
            }
        } catch (JSONException e) {
            Log.d(TAG, "couldn't parse params: "+params);
            return;
        }
        List<String> publishPermissions = new ArrayList<String>();
        List<String> readPermissions = new ArrayList<String>();
        if(parts != null && parts.length > 0) {
            for(String s:parts) {
                if(Session.isPublishPermission(s)) {
                    publishPermissions.add(s);
                } else {
                    readPermissions.add((s));
                }
            }
        }

        boolean hasPublishPermissions = !publishPermissions.isEmpty();
        if (session != Session.getActiveSession()) {
            Session.setActiveSession(session);
        }

        // check to see if the readPermissions have been TOSed already
        // we don't need to show the readPermissions dialog if they have all been TOSed even though it's a mix
        // of permissions
        boolean showMixedPermissionsFlow = hasPublishPermissions && !session.getPermissions().containsAll(readPermissions);

        // if we're logging in and showing a mix of publish and read permission, we need to split up the dialogs
        // first just show the read permissions, then call initAndLogin() with just the publish permissions
        if (showMixedPermissionsFlow) {
            String publish_permissions = TextUtils.join(",", publishPermissions.toArray());
            try {
                unity_params.put("scope", publish_permissions);
            } catch (JSONException e) {
                // should never happen
                Log.d(TAG, "couldn't add back the publish permissions " + publish_permissions);
                return;
            }
            final String only_publish_params = unity_params.toString();

            Session.StatusCallback afterReadPermissionCallback = new Session.StatusCallback() {
                // callback when session changes state
                @Override
                public void call(Session session, SessionState state, Exception exception) {

                    if (!session.isOpened() && state != SessionState.CLOSED_LOGIN_FAILED) {
                        return;
                    }
                    session.removeCallback(this); // without this, the callback will loop infinitely

                    // if someone cancels on the read permissions and we don't even have the most basic access_token
                    // for basic info, we shouldn't be asking for publish permissions.  It doesn't make sense
                    // and it simply won't work anyways.
                    if (session.getAccessToken() == null || session.getAccessToken().equals("")) {
                        unityMessage.putCancelled();
                        unityMessage.send();
                        return;
                    }

                    initAndLogin(only_publish_params, show_login_dialog);
                }
            };

            if (session.isOpened()) {
                session.requestNewReadPermissions(getNewPermissionsRequest(session, afterReadPermissionCallback, readPermissions));
            } else {
                session.openForRead(getOpenRequest(afterReadPermissionCallback, readPermissions));
            }

            return;
        }

        Session.StatusCallback finalCallback = new Session.StatusCallback() {
            // callback when session changes state
            @Override
            public void call(Session session, SessionState state, Exception exception) {

                if (!session.isOpened() && state != SessionState.CLOSED_LOGIN_FAILED) {
                    return;
                }
                session.removeCallback(this);

                if (session.isOpened()) {
                    unityMessage.put("opened", true);
                } else if (state == SessionState.CLOSED_LOGIN_FAILED) {
                    unityMessage.putCancelled();
                }

                if (session.getAccessToken() == null || session.getAccessToken().equals("")) {
                    unityMessage.send();
                    return;
                }

                // there's a chance a subset of the permissions were allowed even if the login was cancelled
                // if the access token is there, try to get it anyways

                FB.setSession(session);
                unityMessage.put("access_token", session.getAccessToken());
                Request.executeMeRequestAsync(session, new Request.GraphUserCallback() {
                    @Override
                    public void onCompleted(GraphUser user, Response response) {
                        if (user != null) {
                            unityMessage.put("user_id", user.getId());
                        }
                        unityMessage.send();
                    }
                });
            }
        };

        if (session.isOpened()) {
            Session.NewPermissionsRequest req = getNewPermissionsRequest(session, finalCallback, (hasPublishPermissions) ? publishPermissions : readPermissions);
            if (hasPublishPermissions) {
                session.requestNewPublishPermissions(req);
            } else {
                session.requestNewReadPermissions(req);
            }
        } else {
            OpenRequest req = getOpenRequest(finalCallback, (hasPublishPermissions) ? publishPermissions : readPermissions);
            if (hasPublishPermissions) {
                session.openForPublish(req);
            } else {
                session.openForRead(req);
            }
        }
	}

    private static OpenRequest getOpenRequest(StatusCallback callback, List<String> permissions) {
        OpenRequest req = new OpenRequest(getActivity());
        req.setCallback(callback);
        req.setPermissions(permissions);
        req.setDefaultAudience(SessionDefaultAudience.FRIENDS);

        return req;
    }

    private static Session.NewPermissionsRequest getNewPermissionsRequest(Session session, StatusCallback callback, List<String> permissions) {
        Session.NewPermissionsRequest req = new Session.NewPermissionsRequest(getActivity(), permissions);
        req.setCallback(callback);
        // This should really be "req.setCallback(callback);"
        // Unfortunately the current underlying SDK won't add the callback when you do it that way
        // TODO: when upgrading to the latest see if this can be "req.setCallback(callback);"
        // if it still doesn't have it, file a bug!
        session.addCallback(callback);
        req.setDefaultAudience(SessionDefaultAudience.FRIENDS);
        return req;
    }

	@UnityCallable
	public static void Init(String params) {
        FB.intent = getActivity().getIntent();
		// tries to log the user in if they've already TOS'd the app
		initAndLogin(params, /*show_login_dialog=*/false);
	}

	@UnityCallable
	public static void Login(String params) {
		initAndLogin(params, /*show_login_dialog=*/true);
	}

	@UnityCallable
	public static void Logout(String params) {
		Session.getActiveSession().closeAndClearTokenInformation();
		new UnityMessage("OnLogoutComplete").send();
	}

	@UnityCallable
    public static void AppRequest(String params_str) {
        Log.v(TAG, "sendRequestDialog(" + params_str + ")");
        final Bundle params = new Bundle();
        final UnityMessage response = new UnityMessage("OnAppRequestsComplete");

        if (!isLoggedIn()) {
            response.sendNotLoggedInError();
            return;
        }

        final JSONObject unity_params;
        try {
            unity_params = new JSONObject(params_str);
            if (!unity_params.isNull("callback_id")) {
                response.put("callback_id", unity_params.getString("callback_id"));
            }
        } catch (JSONException e) {
            response.sendError("couldn't parse params: "+params_str);
            return;
        }

        Iterator<?> keys = unity_params.keys();
        while(keys.hasNext()) {
            String key = (String)keys.next();
            try {
                if (key.equals("callback_id")) {
                    continue;
                }
                String value = unity_params.getString(key);
                if (value != null) {
                    params.putString(key, value);
                }
            } catch (JSONException e) {
                response.sendError("error getting value for key "+key+": "+e.toString());
                return;
            }
        }

        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // TODO Auto-generated method stub
                WebDialog requestsDialog = (
                        new WebDialog.RequestsDialogBuilder(getActivity(),
                                Session.getActiveSession(),
                                params))
                                .setOnCompleteListener(new OnCompleteListener() {

                                    @Override
                                    public void onComplete(Bundle values,
                                            FacebookException error) {

                                        if (error != null) {
                                            if(error.toString().equals("com.facebook.FacebookOperationCanceledException")) {
                                                response.putCancelled();
                                                response.send();
                                            } else {
                                                response.sendError(error.toString());
                                            }
                                        } else {
                                            if(values != null) {
                                                final String requestId = values.getString("request");
                                                if(requestId == null) {
                                                    response.putCancelled();
                                                }
                                            }

                                            for (String key : values.keySet()) {
                                                response.put(key, values.getString(key));
                                            }
                                            response.send();
                                        }

                                    }

                                })
                                .build();
                requestsDialog.show();

            }
        });
    }

	@UnityCallable
	public static void FeedRequest(String params_str) {
		Log.v(TAG, "FeedRequest(" + params_str + ")");
		final UnityMessage response = new UnityMessage("OnFeedRequestComplete");
		final JSONObject unity_params;
		try {
			unity_params = new JSONObject(params_str);
		} catch (JSONException e) {
			response.sendError("couldn't parse params: "+params_str);
			return;
		}

		if (!isLoggedIn()) {
			response.sendNotLoggedInError();
			return;
		}

		getActivity().runOnUiThread(new Runnable() {

            @Override
            public void run() {
                Bundle params = new Bundle();
                Iterator<?> keys = unity_params.keys();
                while (keys.hasNext()) {
                    String key = (String) keys.next();
                    try {
                        String value = unity_params.getString(key);
                        if (value != null) {
                            params.putString(key, value);
                        }
                    } catch (JSONException e) {
                        response.sendError("error getting value for key " + key + ": " + e.toString());
                        return;
                    }
                }

                WebDialog feedDialog = (
                        new WebDialog.FeedDialogBuilder(getActivity(),
                                Session.getActiveSession(),
                                params))
                        .setOnCompleteListener(new OnCompleteListener() {

                            @Override
                            public void onComplete(Bundle values,
                                                   FacebookException error) {

                                // response
                                if (error == null) {
                                    final String postID = values.getString("post_id");
                                    if (postID != null) {
                                        response.putID(postID);
                                    } else {
                                        response.putCancelled();
                                    }
                                    response.send();
                                } else if (error instanceof FacebookOperationCanceledException) {
                                    // User clicked the "x" button
                                    response.putCancelled();
                                    response.send();
                                } else {
                                    // Generic, ex: network error
                                    response.sendError(error.toString());
                                }
                            }

                        })
                        .build();
                feedDialog.show();
            }
        });
	}

	@UnityCallable
	public static void PublishInstall(String params_str) {
		final UnityMessage unityMessage = new UnityMessage("OnPublishInstallComplete");
		final JSONObject unity_params;
		try {
			unity_params = new JSONObject(params_str);
			if (!unity_params.isNull("callback_id")) {
				unityMessage.put("callback_id", unity_params.getString("callback_id"));
			}
			Settings.publishInstallAsync(getActivity().getApplicationContext(), unity_params.getString("app_id"), new Request.Callback() {

				@Override
				public void onCompleted(Response response) {
					if(response.getError() != null) {
						unityMessage.sendError(response.getError().toString());
					} else {
						unityMessage.send();
					}

				}
			});
		} catch (JSONException e) {
			unityMessage.sendError("couldn't parse params: " + params_str);
			return;
		}
	}

    @UnityCallable
    public static void GetDeepLink(String params_str) {
        final UnityMessage unityMessage = new UnityMessage("OnGetDeepLinkComplete");

        Uri targetUri = intent.getData();
        if (targetUri != null) {
            unityMessage.put("deep_link", targetUri.toString());
        } else {
            unityMessage.put("deep_link", "");
        }
        unityMessage.send();
    }

    public static void SetIntent(Intent intent) {
        FB.intent = intent;
        GetDeepLink("");
    }

    public static void SetLimitEventUsage(String params) {
        AppEventsLogger.setLimitEventUsage(getActivity().getApplicationContext(), Boolean.valueOf(params));
    }

    @UnityCallable
    public static void AppEvent(String params) {
        Log.v(TAG, "AppEvent(" + params + ")");
        JSONObject unity_params;
        try {
            unity_params = new JSONObject(params);

            Bundle parameters = new Bundle();
            if (!unity_params.isNull("parameters")) {
                JSONObject unity_params_parameter = unity_params.getJSONObject("parameters");
                for (Iterator<?> keys = unity_params_parameter.keys(); keys.hasNext();) {
                    String key = (String) keys.next();
                    parameters.putString(key,unity_params_parameter.getString(key));
                }
            }

            if (!unity_params.isNull("logPurchase")) {
                FB.getAppEventsLogger().logPurchase(
                        new BigDecimal(unity_params.getDouble("logPurchase")),
                        Currency.getInstance(unity_params.getString("currency")),
                        parameters
                );
            } else if (!unity_params.isNull("logEvent")) {
                FB.getAppEventsLogger().logEvent(
                        unity_params.getString("logEvent"),
                        unity_params.optDouble("valueToSum"),
                        parameters
                );
            } else {
                Log.e(TAG, "couldn't logPurchase or logEvent params: "+params);
            }
        } catch (JSONException e) {
            Log.e(TAG, "couldn't parse params: "+params);
            return;
        }

    }

    /**
     * Provides the key hash to solve the openSSL issue with Amazon
     * @return key hash
     */
    private static String getKeyHash() {
        try {
            PackageInfo info = getActivity().getPackageManager().getPackageInfo(
                getActivity().getPackageName(), PackageManager.GET_SIGNATURES);
            for (Signature signature : info.signatures){
                MessageDigest md = MessageDigest.getInstance("SHA");
                md.update(signature.toByteArray());
                String keyHash = Base64.encodeToString(md.digest(), Base64.DEFAULT);
                Log.d(TAG, "KeyHash: " + keyHash);
                return keyHash;
            }
        } catch (NameNotFoundException e) {
        } catch (NoSuchAlgorithmException e) {
        }
        return "";
    }
}
