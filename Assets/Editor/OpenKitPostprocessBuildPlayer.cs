using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;

public class OpenKitPostprocessBuildPlayer : MonoBehaviour {
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
#if UNITY_IOS
		Process proc = new Process();
		proc.EnableRaisingEvents=false; 
		proc.StartInfo.FileName = Application.dataPath + "/Plugins/OpenKit/PostbuildScripts/PostBuildOpenKitIOSScript";
		proc.StartInfo.Arguments = "'" + pathToBuiltProject + "'";
		proc.Start();
		proc.WaitForExit();
		UnityEngine.Debug.Log("OpenKit iOS build log file: " + System.IO.Directory.GetCurrentDirectory() + "/OpenKitIOSBuildLogFile.txt");
		UnityEngine.Debug.Log("OpenKit path to built project: " + pathToBuiltProject);
#endif
    }
}
