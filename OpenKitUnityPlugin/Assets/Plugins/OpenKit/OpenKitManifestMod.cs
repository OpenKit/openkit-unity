﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;

// Adopted from the Facebook Unity SDK
// https://developers.facebook.com/docs/unity/

namespace UnityEditor.OpenKitEditor
{
	public class OpenKitManifestMod
	{
		public const string OKLoginActivity = "";
		public const string OKLoginTheme = "@style/Theme.Transparent";
		public const string OKLeaderboardActivity = "io.openkit.leaderboards.OKLeaderboardsActivity";
		public const string OKScoresActivity = "io.openkit.leaderboards.OKScoresActivity";
		public const string OKUserProfileActivity = "io.openkit.user.OKUserProfileActivity";


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
			string fbAppId = OKSettings.FacebookAppId;


			if(string.IsNullOrEmpty(fbAppId)) {
				Debug.LogError("You didn't specify a Facebook app ID.  Please add one using the OpenKit config settings in the Window menu in the Unity editor.");
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

			
			//add the Facebook login activity -- The facebook SDK will also add this but just including it here
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

			//Add the OpenKit Login Activity
			XmlElement okLoginElement = FindElementWithAndroidName("activity", "name", ns, OKLoginActivity, dict);
			if (loginElement == null)
			{
				okLoginElement = doc.CreateElement("activity");
				okLoginElement.SetAttribute("name", ns, OKLoginActivity);
				okLoginElement.SetAttribute("theme", ns, OKLoginTheme);
				okLoginElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
				dict.AppendChild(okLoginElement);
			}

			// Add the other OpenKit activities
			AddActivityToManifestWithName(OKLeaderboardActivity,doc,ns,dict);
			AddActivityToManifestWithName(OKScoresActivity,doc,ns,dict);
			AddActivityToManifestWithName(OKUserProfileActivity,doc,ns,dict);

			// Add the Facebook App ID
			//<meta-data android:name="com.facebook.sdk.ApplicationId" android:value="\ 409682555812308" />
			XmlElement appIdElement = FindElementWithAndroidName("meta-data", "name", ns, "com.facebook.sdk.ApplicationId", dict);
			if (appIdElement == null)
			{
				appIdElement = doc.CreateElement("meta-data");
				appIdElement.SetAttribute("name", ns, "com.facebook.sdk.ApplicationId");
				dict.AppendChild(appIdElement);
			}
			appIdElement.SetAttribute("value", ns, "\\ " + fbAppId); //stupid hack so that the id comes out as a string

			
			doc.Save(fullPath);
		}

		private static void AddActivityToManifestWithName(string activityName, XmlDocument doc, string ns, XmlNode dict)
		{
			XmlElement activityElement = FindElementWithAndroidName("activity", "name", ns, activityName, dict);
			if (activityElement == null)
			{
				activityElement = doc.CreateElement("activity");
				activityElement.SetAttribute("name", ns, activityName);
				activityElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
				dict.AppendChild(activityElement);
			}
		}

	}
}