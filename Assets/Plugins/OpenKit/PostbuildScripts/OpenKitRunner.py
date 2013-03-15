#!/usr/bin/python

import sys
import shutil
import os.path

from mod_pbxproj import XcodeProject

def logToBuildLog(x):
    f = open('OpenKitIOSBuildLogFile.txt','a')
    f.write(x + '\n')
    f.close()


logToBuildLog('Start of OpenKit iOS Python build script \n')

projectPath = sys.argv[1]


#debug print out variable paths etc
logToBuildLog('argv1 projectPath:' + projectPath)
logToBuildLog('argv2:' + sys.argv[2])
logToBuildLog('argv3 fbID: ' + sys.argv[3])
logToBuildLog('argv4 openkit resources: ' + sys.argv[4] + '\n')

fb_app_id = sys.argv[3]

#Open and read the Info.plist file
plist_file_path = os.path.join(projectPath, 'Info.plist')
plist_file = open(plist_file_path, 'r')
plist = plist_file.read()
plist_file.close()

logToBuildLog('Opened and read the plist file \n')

elements_to_add = '''
<key>FacebookAppID</key>
<string>''' + fb_app_id + '''</string>
<key>CFBundleURLTypes</key>
<array>
 <dict>
  <key>CFBundleURLSchemes</key>
  <array>
   <string>fb''' + fb_app_id + '''</string>
  </array>
 </dict>
</array>
<key>CFBundleLocalizations</key>
<array>
 <string>en</string>
</array>                    
'''

logToBuildLog('Created plist modifications \n')

#put in the new plist additions
plist = plist.replace('<key>', elements_to_add + '<key>', 1)
plist_file = open(plist_file_path, 'w')
plist_file.write(plist)
plist_file.close()

logToBuildLog('Wrote modifications to the plist file \n\n')

#open the xcode project file
project = XcodeProject.Load(projectPath + '/Unity-iPhone.xcodeproj/project.pbxproj')
logToBuildLog('project loaded:\n \n')

#add security framework
project.add_file('System/Library/Frameworks/Security.framework', tree='SDKROOT')

logToBuildLog('added security framework:\n')

#unity already adds the SystemConfiguration framework
#project.add_file('System/Library/SystemConfiguration.framework', tree='SDKROOT')

#add sqllite3 
project.add_file('usr/lib/libsqlite3.0.dylib', tree='SDKROOT')
logToBuildLog('added libsqlite3.0:\n')

#Add Twitter Framework
project.add_file('System/Library/Frameworks/Twitter.framework', tree='SDKROOT')
logToBuildLog('added twitter framework:\n')


#Add QuartzCore Framework
project.add_file('System/Library/Frameworks/QuartzCore.framework', tree='SDKROOT')
logToBuildLog('added QuartzCore framework:\n')


#Add AdSupport Framework
project.add_file('System/Library/Frameworks/AdSupport.framework', tree='SDKROOT')
logToBuildLog('added AdSupport framework:\n')


#Add Accounts Framework
project.add_file('System/Library/Frameworks/Accounts.framework', tree='SDKROOT')
logToBuildLog('added Accounts framework:\n')


#Add Social Framework
project.add_file('System/Library/Frameworks/Social.framework', tree='SDKROOT')
logToBuildLog('added Social framework:\n')


#Add MobileCoreServices Framework
project.add_file('System/Library/Frameworks/MobileCoreServices.framework', tree='SDKROOT')
logToBuildLog('added MobileCoreServices framework:\n\n')



#copy over vendor folder
shutil.copytree(sys.argv[2] , projectPath + '/OpenKit_Vendor');
logToBuildLog('copied over OpenKit_Vendor folder:\n')


#create the vendor group in the xcode project
#vendor_group = project.get_or_create_group('OpenKit_Vendor')
#logToBuildLog('Created vendor group in project');

#add the AFNetworkign folder to vendor group
#project.add_folder(projectPath + '/OpenKit_Vendor/AFNetworking',parent=vendor_group)
#logToBuildLog('Added afnetworking to vendor group')

#add JSONKit to vendor group
#project.add_folder(projectPath + '/OpenKit_Vendor/JSONKit',parent=vendor_group)
#logToBuildLog('Added JSONKIt to vendor group')

#add the Facebook framework
#project.add_file(projectPath + '/OpenKit_Vendor/FacebookSDK.framework',tree='SDKROOT')
#logToBuildLog('Added Facebook framework to vendor group')

#add vendor folder to xcode project
project.add_folder(projectPath + '/OpenKit_Vendor');
logToBuildLog('Added OpenKit vendor folder to project.h:\n')

#copy over resources folder folder
resources_dir = sys.argv[4];
shutil.copytree(resources_dir , projectPath + '/OpenKitResources');
logToBuildLog('copied over OpenKitResources folder:')

#add resources folder to xcode project
project.add_folder(projectPath + '/OpenKitResources');
logToBuildLog('Added OpenKitResources folder to project.h:\n')




#save project
project.saveFormat3_2()
logToBuildLog('Saved project:\n')

