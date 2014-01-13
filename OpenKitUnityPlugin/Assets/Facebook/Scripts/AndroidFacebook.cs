using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook
{
    sealed class AndroidFacebook : AbstractFacebook, IFacebook
    {
        public const int BrowserDialogMode = 0;

        private const string AndroidJavaFacebookClass = "com.facebook.unity.FB";
        private const string CallbackIdKey = "callback_id";

        // key Hash used for Android SDK
        private string keyHash;
        public string KeyHash { get { return keyHash; } }

        #region IFacebook
        public override int DialogMode { get { return BrowserDialogMode; } set { } }
        public override bool LimitEventUsage
        {
            get
            {
                return limitEventUsage;
            }
            set
            {
                limitEventUsage = value;
                CallFB("SetLimitEventUsage", value.ToString());
            }
        }
        #endregion

        private FacebookDelegate deepLinkDelegate;

        #region FBJava

#if UNITY_ANDROID
        private AndroidJavaClass fbJava;
        private AndroidJavaClass FB
        {
            get
            {
                if (fbJava == null)
                {
                    fbJava = new AndroidJavaClass(AndroidJavaFacebookClass);

                    if (fbJava == null)
                    {
                        throw new MissingReferenceException(string.Format("AndroidFacebook failed to load {0} class", AndroidJavaFacebookClass));
                    }
                }
                return fbJava;
            }
        }
#endif
        private void CallFB(string method, string args)
        {
#if UNITY_ANDROID
            FB.CallStatic(method, args);
#else
            FbDebug.Error("Using Android when not on an Android build!  Doesn't Work!");
#endif
        }

        #endregion

        #region FBAndroid

        protected override void OnAwake()
        {
            keyHash = "";
#if DEBUG
            AndroidJNIHelper.debug = true;
#endif
        }

        private bool IsErrorResponse(string response)
        {
            //var res = MiniJSON.Json.Deserialize(response);
            return false;
        }

        private InitDelegate onInitComplete = null;
        public override void Init(
            InitDelegate onInitComplete,
            string appId,
            bool cookie = false,
            bool logging = true,
            bool status = true,
            bool xfbml = false,
            string channelUrl = "",
            string authResponse = null,
            bool frictionlessRequests = false,
            HideUnityDelegate hideUnityDelegate = null)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentException("appId cannot be null or empty!");
            }

            var parameters = new Dictionary<string, object>();

            parameters.Add("appId", appId);

            if (cookie != false)
            {
                parameters.Add("cookie", true);
            }
            if (logging != true)
            {
                parameters.Add("logging", false);
            }
            if (status != true)
            {
                parameters.Add("status", false);
            }
            if (xfbml != false)
            {
                parameters.Add("xfbml", true);
            }
            if (!string.IsNullOrEmpty(channelUrl))
            {
                parameters.Add("channelUrl", channelUrl);
            }
            if (!string.IsNullOrEmpty(authResponse))
            {
                parameters.Add("authResponse", authResponse);
            }
            if (frictionlessRequests != false)
            {
                parameters.Add("frictionlessRequests", true);
            }

            var paramJson = MiniJSON.Json.Serialize(parameters);
            this.onInitComplete = onInitComplete;

            this.CallFB("Init", paramJson.ToString());
        }

        public void OnInitComplete(string message)
        {
            OnLoginComplete(message);
            if (this.onInitComplete != null)
            {
                this.onInitComplete();
            }
        }

        public override void Login(string scope = "", FacebookDelegate callback = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("scope", scope);
            var paramJson = MiniJSON.Json.Serialize(parameters);
            AddAuthDelegate(callback);
            this.CallFB("Login", paramJson);
        }

        public void OnLoginComplete(string message)
        {
            var parameters = (Dictionary<string, object>)MiniJSON.Json.Deserialize(message);

            if (parameters.ContainsKey("user_id"))
            {
                isLoggedIn = true;
                userId = (string)parameters["user_id"];
                accessToken = (string)parameters["access_token"];
            }

            if (parameters.ContainsKey("key_hash"))
            {
                keyHash = (string)parameters["key_hash"];
            }

            OnAuthResponse(new FBResult(message));
        }

        public override void Logout()
        {
            this.CallFB("Logout", "");
        }

        public void OnLogoutComplete(string message)
        {
            isLoggedIn = false;
            userId = "";
            accessToken = "";
        }

        public override void AppRequest(
            string message,
            string[] to = null,
            string filters = "",
            string[] excludeIds = null,
            int? maxRecipients = null,
            string data = "",
            string title = "",
            FacebookDelegate callback = null)
        {
            Dictionary<string, object> paramsDict = new Dictionary<string, object>();
            // Marshal all the above into the thing

            paramsDict["message"] = message;

            if (callback != null)
            {
                paramsDict["callback_id"] = AddFacebookDelegate(callback);
            }

            if (to != null)
            {
                paramsDict["to"] = string.Join(",", to);
            }

            if (!string.IsNullOrEmpty(filters))
            {
                paramsDict["filters"] = filters;
            }

            if (maxRecipients != null)
            {
                paramsDict["max_recipients"] = maxRecipients.Value;
            }

            if (!string.IsNullOrEmpty(data))
            {
                paramsDict["data"] = data;
            }

            if (!string.IsNullOrEmpty(title))
            {
                paramsDict["title"] = title;
            }

            CallFB("AppRequest", MiniJSON.Json.Serialize(paramsDict));
        }

        public void OnAppRequestsComplete(string message)
        {
            var rawResult = (Dictionary<string, object>)MiniJSON.Json.Deserialize(message);
            if (rawResult.ContainsKey(CallbackIdKey))
            {
                var result = new Dictionary<string, object>();
                var callbackId = (string)rawResult[CallbackIdKey];
                rawResult.Remove(CallbackIdKey);
                if (rawResult.Count > 0)
                {
                    List<string> to = new List<string>(rawResult.Count - 1);
                    foreach (string key in rawResult.Keys)
                    {
                        if (!key.StartsWith("to"))
                        {
                            result[key] = rawResult[key];
                            continue;
                        }
                        to.Add((string)rawResult[key]);
                    }
                    result.Add("to", to);
                    rawResult.Clear();
                    OnFacebookResponse(callbackId, new FBResult(MiniJSON.Json.Serialize(result)));
                }
                else
                {
                    //if we make it here java returned a callback message with only an id
                    //this isnt supposed to happen
                    OnFacebookResponse(callbackId, new FBResult(MiniJSON.Json.Serialize(result), "Malformed request response.  Please file a bug with facebook here: https://developers.facebook.com/bugs/create"));
                }
            }
        }

        public override void FeedRequest(
            string toId = "",
            string link = "",
            string linkName = "",
            string linkCaption = "",
            string linkDescription = "",
            string picture = "",
            string mediaSource = "",
            string actionName = "",
            string actionLink = "",
            string reference = "",
            Dictionary<string, string[]> properties = null,
            FacebookDelegate callback = null)
        {
            Dictionary<string, object> paramsDict = new Dictionary<string, object>();
            // Marshal all the above into the thing

            if (!string.IsNullOrEmpty(toId))
            {
                paramsDict.Add("to", toId);
            }

            if (!string.IsNullOrEmpty(link))
            {
                paramsDict.Add("link", link);
            }

            if (!string.IsNullOrEmpty(linkName))
            {
                paramsDict.Add("name", linkName);
            }

            if (!string.IsNullOrEmpty(linkCaption))
            {
                paramsDict.Add("caption", linkCaption);
            }

            if (!string.IsNullOrEmpty(linkDescription))
            {
                paramsDict.Add("description", linkDescription);
            }

            if (!string.IsNullOrEmpty(picture))
            {
                paramsDict.Add("picture", picture);
            }

            if (!string.IsNullOrEmpty(mediaSource))
            {
                paramsDict.Add("source", mediaSource);
            }

            if (!string.IsNullOrEmpty(actionName) && !string.IsNullOrEmpty(actionLink))
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("name", actionName);
                dict.Add("link", actionLink);

                paramsDict.Add("actions", new[] { dict });
            }

            if (!string.IsNullOrEmpty(reference))
            {
                paramsDict.Add("ref", reference);
            }

            if (properties != null)
            {
                Dictionary<string, object> newObj = new Dictionary<string, object>();
                foreach (KeyValuePair<string, string[]> pair in properties)
                {
                    if (pair.Value.Length < 1)
                        continue;

                    if (pair.Value.Length == 1)
                    {
                        // String-string
                        newObj.Add(pair.Key, pair.Value[0]);
                    }
                    else
                    {
                        // String-Object with two parameters
                        Dictionary<string, object> innerObj = new Dictionary<string, object>();

                        innerObj.Add("text", pair.Value[0]);
                        innerObj.Add("href", pair.Value[1]);

                        newObj.Add(pair.Key, innerObj);
                    }
                }
                paramsDict.Add("properties", newObj);
            }

            CallFB("FeedRequest", MiniJSON.Json.Serialize(paramsDict));
        }


        public void OnFeedRequestComplete(string message)
        {
        }

        public override void Pay(
            string product,
            string action = "purchaseitem",
            int quantity = 1,
            int? quantityMin = null,
            int? quantityMax = null,
            string requestId = null,
            string pricepointId = null,
            string testCurrency = null,
            FacebookDelegate callback = null)
        {
            throw new PlatformNotSupportedException("There is no Facebook Pay Dialog on Android");
        }

        public override void GetDeepLink(FacebookDelegate callback)
        {
            if (callback != null)
            {
                deepLinkDelegate = callback;
                CallFB("GetDeepLink", "");
            }
        }

        public void OnGetDeepLinkComplete(string message)
        {
            var rawResult = (Dictionary<string, object>) MiniJSON.Json.Deserialize(message);
            if (deepLinkDelegate != null)
            {
                object deepLink = "";
                rawResult.TryGetValue("deep_link", out deepLink);
                deepLinkDelegate(new FBResult(deepLink.ToString()));
            }
        }

        public override void AppEventsLogEvent(
            string logEvent,
            float? valueToSum = null,
            Dictionary<string, object> parameters = null)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict["logEvent"] = logEvent;
            if (valueToSum.HasValue)
            {
                paramsDict["valueToSum"] = valueToSum.Value;
            }
            if (parameters != null)
            {
                paramsDict["parameters"] = ToStringDict(parameters);
            }
            CallFB("AppEvents", MiniJSON.Json.Serialize(paramsDict));
        }

        public override void AppEventsLogPurchase(
            float logPurchase,
            string currency = "USD",
            Dictionary<string, object> parameters = null)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict["logPurchase"] = logPurchase;
            paramsDict["currency"] = (!string.IsNullOrEmpty(currency)) ? currency : "USD";
            if (parameters != null)
            {
                paramsDict["parameters"] = ToStringDict(parameters);
            }
            CallFB("AppEvents", MiniJSON.Json.Serialize(paramsDict));
        }

        #endregion

        #region Helper Functions

        public override void PublishInstall(string appId, FacebookDelegate callback = null)
        {
            var parameters = new Dictionary<string, string>(2);
            parameters["app_id"] = appId;
            if (callback != null)
            {
                parameters["callback_id"] = AddFacebookDelegate(callback);
            }
            CallFB("PublishInstall", MiniJSON.Json.Serialize(parameters));
        }

        public void OnPublishInstallComplete(string message)
        {
            var response = (Dictionary<string, object>)MiniJSON.Json.Deserialize(message);
            if (response.ContainsKey("callback_id"))
            {
                OnFacebookResponse((string)response["callback_id"], new FBResult(""));
            }
        }

        private Dictionary<string, string> ToStringDict(Dictionary<string, object> dict)
        {
            if (dict == null)
            {
                return null;
            }
            var newDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                newDict[kvp.Key] = kvp.Value.ToString();
            }
            return newDict;
        }

        #endregion
    }
}

