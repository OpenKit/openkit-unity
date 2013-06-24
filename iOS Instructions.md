Instructions for iOS
====================

To get the latest changes from iOS, you need to do the following at a high level:

1) Build latest versions of libOpenKit.a and libUnityPlugin.a

2) Copy them from the build folder "Release-iphoneos" into openkit-unity folder of Assets/Plugins/iOS/

3) Copy all resource files from "include" and "Resources" directory of "Release-iphoneos" into Assets/Plugins/iOS/OpenKitResources


Building latest version & Copying files
=======================================

First verify that everything is being built correctly by running the sample app. 

1) Clean *all* targets, including OpenKit, SampleApp, and UnityPlugin

2) Change schemes for "run" to Release for both OpenKit and UnityPlugin targets

3) Hit run to build OpenKit target

4) Hit run to build UnityPlugin target

5) Open up the "Products" folder in xcode project navigator, you should see libOpenKit.a and libUnityPlugin.a

5) Right click **libOpenKit.a** and select "Show in finder"

6) Copy libOpenKit.a and libUnityPlugin.a into /openkit-unity/Assets/Plugins/iOS

7) Copy "include" and "Resources" folder into /openkit-unity/Assets/Plugins/iOS/OpenKitResources

