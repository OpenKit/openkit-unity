using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Facebook
{
    class EditorFacebook : AbstractFacebook, IFacebook
    {
        private IFacebook fb;

        public override int DialogMode
        {
            get { return 0; }
            set { ; }
        }

        public override bool LimitEventUsage
        {
            get
            {
                return limitEventUsage;
            }
            set
            {
                limitEventUsage = value;
            }
        }

        #region Init
        protected override void OnAwake()
        {
            // bootstrap the canvas facebook for native dialogs
            StartCoroutine(FB.RemoteFacebookLoader.LoadFacebookClass("CanvasFacebook", OnDllLoaded));
        }

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
            Facebook.HideUnityDelegate hideUnityDelegate = null)
        {
            StartCoroutine(OnInit(onInitComplete, appId, cookie, logging, status, xfbml, channelUrl, authResponse, frictionlessRequests, hideUnityDelegate));
        }

        private IEnumerator OnInit(
            InitDelegate onInitComplete,
            string appId,
            bool cookie = false,
            bool logging = true,
            bool status = true,
            bool xfbml = false,
            string channelUrl = "",
            string authResponse = null,
            bool frictionlessRequests = false,
            Facebook.HideUnityDelegate hideUnityDelegate = null)
        {
            // wait until the native dialogs are loaded
            while (fb == null)
            {
                yield return null;
            }
            fb.Init(onInitComplete, appId, cookie, logging, status, xfbml, channelUrl, authResponse, frictionlessRequests, hideUnityDelegate);
            if (status || cookie)
            {
                isLoggedIn = true;
            }
            if (onInitComplete != null)
            {
                onInitComplete();
            }
        }

        private void OnDllLoaded(IFacebook fb)
        {
            this.fb = fb;
        }
        #endregion

        public override void Login(string scope = "", FacebookDelegate callback = null)
        {
            if (isLoggedIn)
            {
                FbDebug.Warn("User is already logged in.  You don't need to call this again.");
            }

            userId = "0";
            accessToken = "abcdefghijklmnopqrstuvwxyz";
            isLoggedIn = true;
        }

        public override void Logout()
        {
            isLoggedIn = false;
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
            fb.AppRequest(message, to, filters, excludeIds, maxRecipients, data, title, callback);
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
            fb.FeedRequest(toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, actionName, actionLink, reference, properties, callback);
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
            FbDebug.Info("Pay method only works with Facebook Canvas.  Does nothing in the Unity Editor, iOS or Android");
        }

        public override void API(
            string query,
            HttpMethod method,
            Dictionary<string, string> formData = null,
            FacebookDelegate callback = null)
        {
            if (query.StartsWith("me"))
            {
                FbDebug.Warn("graph.facebook.com/me does not work within the Unity Editor");
            }

            if (!query.Contains("access_token=") && (formData == null || !formData.ContainsKey("access_token")))
            {
                FbDebug.Warn("Without an access_token param explicitly passed in formData, some API graph calls will 404 error in the Unity Editor.");
            }
            fb.API(query, method, formData, callback);
        }

        public override void GetAuthResponse(FacebookDelegate callback = null)
        {
            fb.GetAuthResponse(callback);
        }
        
        public override void PublishInstall(string appId, FacebookDelegate callback = null) {}

        public override void GetDeepLink(FacebookDelegate callback)
        {
            FbDebug.Info("No Deep Linking in the Editor");
            if (callback != null)
            {
                callback(new FBResult("<platform dependent>"));
            }
        }

        public override void AppEventsLogEvent(
            string logEvent,
            float? valueToSum = null,
            Dictionary<string, object> parameters = null)
        {
            FbDebug.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
        }

        public override void AppEventsLogPurchase(
            float logPurchase,
            string currency = "USD",
            Dictionary<string, object> parameters = null)
        {
            FbDebug.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
        }
    }
}
