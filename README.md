Important
=========

After cloning the repo, run the following command from the root directory: 

```
$ ./link.pl
```


Tips
====
* When you build and run, please make the build directory "Xcode", as that is
included in the .gitignore.

* To see OpenKit output when running in the Unity Editor: `$ tail -f ~/Library/Logs/Unity/Editor.log | grep OpenKit`


Integrate OpenKit into your Unity Project
=========================================

OpenKit and Other Unity Plugins
--------------------------------

If you have other Unity native plugins, you may need to do some work to make them work together. 

The OpenKit Unity plugin uses native plugins for both iOS and Android.

Android
---------
The OpenKit android plugin files are located in Assets/Plugins/Android/. The res folder contains all the resources that the OpenKit plugin needs.

If you have another Android plugin that declares resources, you will need to merge the contents of OpenKit's res folder and the other plugins' res folders.

Merging the files will be straightforward, but be sure to not replace any files. Specifically, you will need to merge the contents of the following files together for all your plugins:

	Plugins/Android/res/values/attrs.xml
	Plugins/Android/res/values/colors.xml
	Plugins/Android/res/values/strings.xml
	Plugins/Android/res/values/styles.xml
	
	
	Plugins/Android/res/values-v11/styles.xml
	Plugins/Android/res/values-v14/styles.xml
	
iOS
----------

The OpenKit iOS plugin uses PostBuildScripts to configure the generated XCode project correctly. If you have other plugins that use PostBuildScripts, you need to make sure they all run correctly. 


1. Import the OpenKit unity package

	Be sure to select all files, and import the package.

2. Create a Facebook application 

	Follow the guide here to create a Facebook application: http://openkit.io/documentation/#facebook

	You need a Facebook application to try out OpenKit authentication. Note your Facebook application ID. 

	If you're testing on Android, be sure to get your Android keyhash and add it. 

3. Set your bundle identifier in Unity Player Settings 

	In Unity Player settings, set your bundle identifier for both Android and iOS.

	Make sure your Facebook application is updated with these settings for both iOS and Android. 

4. Enter your Facebook app ID in the relevant places

	You need to enter your Facebook App ID in two places:

	Android:

	Open up Assets/Plugins/Android/AndroidManifest.xml and find the lines:

		<!-- Metadata tag required by facebook SDK. -->
		<!-- Be sure to replace the ID number below in the android:value attribute wth your own Facebook App ID! -->
		<!-- You MUST keep the SLASH inside the quotes, e.g. android:value="\ YOUR_FB_APP_ID"   or your APP WILL CRASH! -->
	
		<meta-data
	       	android:name="com.facebook.sdk.ApplicationId"
	       	android:value="\ FB_APP_ID"></meta-data>
	
		Replace FB_APP_ID with your app id. As the note says, BE SURE to keep the SLASH inside the quotes.

	iOS:

		Open up the file **Assets/Plugins/Editor/OpenKitPostprocessBuildPlayer.cs** and find the line:

			public static string FacebookAppID = "450333868362300";

		Replace this with your facebook app id.

5. Set your OpenKit app key in the start of your game

	Open up the file **OKInitializer.cs** found in Assets/Plugins/OpenKit and enter your AppKey and SecretKey in the initializer. In your game's main scene, where your game first starts, add the OpenKit prefab found in Assets/Prefabs .

	The OpenKit prefab will call OKInitializer.cs and initialize OpenKit. If you want to initialize OpenKit yourself without the prefab, simply instantiate OKInitializer somewhere in your code.

	Make sure that you are calling OKManager.Configure(appKey,secretKey) when your game starts.

6. Check out the  OpenKit Demo Scene and Script

	There is a demo scene that shows OpenKit login, leaderboards, and submitting scores copied into **Assets/**, called **OKDemoScene** with a corresponding script, **OKDemoScene.cs** .

	Add this scene and build and run on iOS and Android to try it out. 

	When running the demo scene, for Facebook authentication to work properly on both iOS and Android, you need to create your own Facebook application and use your own application ID. 

7. Build your Unity app

	Be sure to create your build from the "Build Settings" window. This option ensures that all scripts and project files will be generated correctly.

	If you use the "Build and Run" option from the File menu, some actions may be skipped. 

	When building for iOS, you should choose "Replace" as opposed to "Append" when generating the XCode project. 
	
