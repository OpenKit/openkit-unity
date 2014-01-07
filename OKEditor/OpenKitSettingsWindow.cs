using System;
using UnityEditor;
using UnityEngine;

public class OpenKitSettingsWindow : EditorWindow
{
	[MenuItem("Window/OpenKit/Config")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<OpenKitSettingsWindow>("OpenKit Config");
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
		if (GUILayout.Button("Apply"))
			OKSettings.Save();
	}
}
