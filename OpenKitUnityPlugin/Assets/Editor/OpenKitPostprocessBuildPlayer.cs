using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.IO;

public class OpenKitPostprocessBuildPlayer : MonoBehaviour {

	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {

		OKSettings.Load();
		if (string.IsNullOrEmpty(OKSettings.FacebookAppId)) {
			UnityEngine.Debug.LogError("Missing Facebook App Id for OpenKit - add it using the OpenKit config menu to configure your app before deploying!");
		}
		
		if(string.IsNullOrEmpty(OKSettings.AppKey) || string.IsNullOrEmpty(OKSettings.AppSecretKey)) {
			UnityEngine.Debug.LogError("Missing AppKey or SecretKey for OpenKit. You must add it using the OpenKit config menu to configure your app before building");
			throw new System.ArgumentException("Missing appKey or secretKey for OpenKit");
		}

#if UNITY_IOS
		string f = "OpenKitIOSBuildLogFile.txt";
		string logfile = System.IO.Directory.GetCurrentDirectory() + "/" + f;
		UnityEngine.Debug.Log("Logging OpenKit post build to " + f);
		LogTo(logfile, "In Editor/OpenKitPostProcessBuildPlayer.cs\n");

		Process proc = new Process();
		proc.EnableRaisingEvents=false;
		proc.StartInfo.FileName = Application.dataPath + "/Plugins/OpenKit/PostbuildScripts/PostBuildOpenKitIOSScript";
		proc.StartInfo.Arguments = "'" + pathToBuiltProject + "' '" + OKSettings.FacebookAppId + "'";
		
		// Add the Unity version as an argument to the postbuild script, use 'Unity3' for all 3.x versions and for
		// 4 and up use the API to get it
		string unityVersion;
#if UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0
		unityVersion = "Unity3";
#else
		unityVersion = Application.unityVersion;
#endif
		proc.StartInfo.Arguments += " '" + unityVersion + "'";
		proc.Start();
		proc.WaitForExit();
#endif
	}
	public static void LogTo(string logfile, string message)
	{
		using (StreamWriter w = File.AppendText(logfile)) {
		  System.DateTime date = System.DateTime.Now;
		  w.WriteLine("{0:yyyy/MM/dd H:mm:ss}: {1}", date, message);
		}
	}
}
