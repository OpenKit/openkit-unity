using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;

namespace UnityEditor.FacebookEditor
{
    public class ManifestMod
    {
        public const string ActivityName = "com.facebook.unity.FBUnityPlayerActivity";

        public static void GenerateManifest()
        {
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

            // only copy over a fresh copy of the AndroidManifest if one does not exist
            if (!File.Exists(outputFile))
            {
                var inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/androidplayer/AndroidManifest.xml");
                File.Copy(inputFile, outputFile);
            }
            UpdateManifest(outputFile);
        }

        private static XmlNode FindChildNode(XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        private static XmlElement FindMainActivityNode(XmlNode parent)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals("activity") && curr.FirstChild != null && curr.FirstChild.Name.Equals("intent-filter"))
                {
                    return curr as XmlElement;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        private static XmlElement FindElementWithAndroidName(string name, string androidName, string ns, string value, XmlNode parent)
        {
            var curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name) && curr is XmlElement && ((XmlElement)curr).GetAttribute(androidName, ns) == value)
                {
                    return curr as XmlElement;
                }
                curr = curr.NextSibling;
            }
            return null;
        }


        public static void UpdateManifest(string fullPath)
        {
            string appId = FBSettings.AppId;

            if (!FBSettings.IsValidAppId)
            {
                Debug.LogError("You didn't specify a Facebook app ID.  Please add one using the Facebook menu in the main Unity editor.");
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(fullPath);

            if (doc == null)
            {
                Debug.LogError("Couldn't load " + fullPath);
                return;
            }

            XmlNode manNode = FindChildNode(doc, "manifest");
            XmlNode dict = FindChildNode(manNode, "application");

            if (dict == null)
            {
                Debug.LogError("Error parsing " + fullPath);
                return;
            }

            string ns = dict.GetNamespaceOfPrefix("android");

            //change 
            //<activity android:name="com.unity3d.player.UnityPlayerProxyActivity" android:launchMode="singleTask" android:label="@string/app_name" android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen" android:screenOrientation="portrait">
            //to
            //<activity android:name="com.facebook.unity.FBUnityPlayerActivity" android:launchMode="singleTask" android:label="@string/app_name" android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen" android:screenOrientation="portrait">
            XmlElement mainActivity = FindMainActivityNode(dict);
            var mainActivityName = mainActivity.GetAttribute("name", ns);
            if (mainActivityName != "com.unity3d.player.UnityPlayerProxyActivity" && 
                mainActivityName != "com.unity3d.player.UnityPlayerNativeActivity" && 
                mainActivityName != ActivityName)
            {
                FbDebug.Warn("FBUnityPlayerActivity was not detected as the main activity in the AndroidManifest.xml!  Be sure to have your activity extend " + ActivityName + " for the Facebook SDK to work");
            }
            else
            {
                mainActivity.SetAttribute("name", ns, ActivityName);
            }

            //add the login activity
            //<activity android:name="com.facebook.LoginActivity" android:screenOrientation="portrait" android:configChanges="keyboardHidden|orientation">
            //</activity>
            XmlElement loginElement = FindElementWithAndroidName("activity", "name", ns, "com.facebook.LoginActivity", dict);
            if (loginElement == null)
            {
                loginElement = doc.CreateElement("activity");
                loginElement.SetAttribute("name", ns, "com.facebook.LoginActivity");
                loginElement.SetAttribute("screenOrientation", ns, "portrait");
                loginElement.SetAttribute("configChanges", ns, "keyboardHidden|orientation");
                loginElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
                dict.AppendChild(loginElement);
            }

            //add the app id
            //<meta-data android:name="com.facebook.sdk.ApplicationId" android:value="\ 409682555812308" />
            XmlElement appIdElement = FindElementWithAndroidName("meta-data", "name", ns, "com.facebook.sdk.ApplicationId", dict);
            if (appIdElement == null)
            {
                appIdElement = doc.CreateElement("meta-data");
                appIdElement.SetAttribute("name", ns, "com.facebook.sdk.ApplicationId");
                dict.AppendChild(appIdElement);
            }
            appIdElement.SetAttribute("value", ns, "\\ " + appId); //stupid hack so that the id comes out as a string

            doc.Save(fullPath);
        }
    }
}