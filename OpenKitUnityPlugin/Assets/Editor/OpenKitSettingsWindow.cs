using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.OpenKitEditor;

public class OpenKitSettingsWindow : EditorWindow
{
	[MenuItem("Window/OpenKit/Config")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<OpenKitSettingsWindow>("OpenKit Config");
	}

	[MenuItem("Window/OpenKit/Update Android Manifest")]
	public static void UpdateManifest()
	{
		UnityEditor.OpenKitEditor.OpenKitManifestMod.GenerateManifest();
	}

	private void OnEnable()
	{
		OKSettings.Load();
	}

	private void OnGUI()
	{
		OKSettings.AppKey = EditorGUILayout.TextField("OpenKit App Key", OKSettings.AppKey);
		OKSettings.AppSecretKey = EditorGUILayout.TextField("OpenKit Secret Key", OKSettings.AppSecretKey);
		OKSettings.FacebookAppId = EditorGUILayout.TextField("Facebook App Id", OKSettings.FacebookAppId);
		if (GUILayout.Button("Apply")) {
			OKSettings.Save();
			OpenKitManifestMod.GenerateManifest();
		}
	}
}
