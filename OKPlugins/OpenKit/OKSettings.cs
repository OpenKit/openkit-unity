using System;
using UnityEngine;

/// <summary>
/// Manges the storage and retrieval of OpenKit settings for an individual application. This class intelligently adapts to use in the editor vs. use at runtime for end users
/// (e.g. settings are readonly and loaded as resources in deployed code, etc.) Should work on *all* platforms.
/// </summary>
public static class OKSettings
{
	private static string appKey, appSecretKey, facebookAppId;
	private const string errorMsgFailedToLoad = "Failed to load OpenKit settings - you must configure OpenKit in the Unity Editor by using the OpenKit menu before deploying builds!";
	private const string errorMsgCorrupt = "OpenKit settings file is corrupt - you must configure OpenKit in the Unity Editor by using the OpenKit menu before deploying builds!";
	private const string persistentFilename = "OKSettings";

	public static string AppKey
	{
		get { return appKey; }
#if UNITY_EDITOR
		set { appKey = value; }
#endif
	}
	
	public static string AppSecretKey
	{
		get { return appSecretKey; }
#if UNITY_EDITOR
		set { appSecretKey = value; }
#endif
	}
	
	public static string FacebookAppId
	{
		get { return facebookAppId; }
#if UNITY_EDITOR
		set { facebookAppId = value; }
#endif
	}

	public static void Load()
	{
#if UNITY_EDITOR
		if (System.IO.File.Exists(PersistentFilename))
		{
			string settings = System.IO.File.ReadAllText(PersistentFilename);
			ParseSettings(settings);
		}
#else
		TextAsset settings = Resources.Load(persistentFilename) as TextAsset;
		if (settings == null)
			Debug.LogError(errorMsgFailedToLoad);
		else
			ParseSettings(settings.text);
#endif
	}

#if UNITY_EDITOR
	private static string PersistentFilename
	{
		get
		{
			string filename = System.IO.Path.Combine("Assets", "Resources");
			if (!System.IO.Directory.Exists(filename))
				System.IO.Directory.CreateDirectory(filename);
			filename = System.IO.Path.Combine(filename, persistentFilename);
			return filename + ".txt";
		}
	}

	public static void Save()
	{
		string settings = String.Format("{0}\n{1}\n{2}", appKey, appSecretKey, facebookAppId);
		System.IO.File.WriteAllText(PersistentFilename, settings);
	}
#endif

	private static void ParseSettings(string settings)
	{
#if !UNITY_EDITOR
		if (settings == null)
			Debug.LogError(errorMsgFailedToLoad);
#endif
		if (settings != null)
		{
			if (String.IsNullOrEmpty(settings))
				Debug.LogError(errorMsgCorrupt);
			else
			{
				string[] lines = settings.Split(new char[] { '\n' });
				if (lines.Length != 3)
					Debug.LogError(errorMsgCorrupt);
				else
				{
					appKey = lines[0];
					appSecretKey = lines[1];
					facebookAppId = lines[2];
				}
			}
		}
	}
}
