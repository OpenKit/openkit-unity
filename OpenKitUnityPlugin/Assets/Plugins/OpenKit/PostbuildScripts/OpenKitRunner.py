#!/usr/bin/python

import sys
import shutil
import os.path
from mod_pbxproj import XcodeProject

def log(x):
  with open('OpenKitIOSBuildLogFile.txt','a') as f:
    f.write(x + "\n")

log('In OpenKitRunner.py\n'
    '---------------------------')

install_path      = sys.argv[1]
fb_app_id         = sys.argv[2]
ok_vendor_path    = sys.argv[3]
ok_resources_path = sys.argv[4]

log('Install path: '      + install_path      + '\n'
    'Facebook app ID: '   + fb_app_id         + '\n'
    'OK Vendor Path: '    + ok_vendor_path    + '\n'
    'OK Resources Path: ' + ok_resources_path)

plist_file_path = os.path.join(install_path, 'Info.plist')
plist_file = open(plist_file_path, 'r')
plist = plist_file.read()
plist_file.close()
log('Opened and read contents of Info.plist.')

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
<key>UIViewControllerBasedStatusBarAppearance</key>
<false/>
'''

plist = plist.replace('<key>', elements_to_add + '<key>', 1)
plist_file = open(plist_file_path, 'w')
plist_file.write(plist)
plist_file.close()
log('Wrote modifications to Info.plist.')

project = XcodeProject.Load(install_path + '/Unity-iPhone.xcodeproj/project.pbxproj')
log('Loaded project.pbxproj.')

project.add_file('System/Library/Frameworks/MessageUI.framework', tree='SDKROOT')
log('Added security framework.')

project.add_file('System/Library/Frameworks/Security.framework', tree='SDKROOT')
log('Added security framework.')

project.add_file('usr/lib/libsqlite3.0.dylib', tree='SDKROOT')
log('Added libsqlite3.0.')

#project.add_file('System/Library/Frameworks/Twitter.framework', tree='SDKROOT', weak=True)
#log('Added twitter framework.')

project.add_file('System/Library/Frameworks/QuartzCore.framework', tree='SDKROOT')
log('Added QuartzCore framework.')

project.add_file('System/Library/Frameworks/AdSupport.framework', tree='SDKROOT', weak=True)
log('Added AdSupport framework.')

project.add_file('System/Library/Frameworks/Accounts.framework', tree='SDKROOT', weak=True)
log('Added Accounts framework.')

project.add_file('System/Library/Frameworks/Social.framework', tree='SDKROOT', weak=True)
log('Added Social framework.')

project.add_file('System/Library/Frameworks/MobileCoreServices.framework', tree='SDKROOT')
log('Added MobileCoreServices framework.')

shutil.copytree(ok_vendor_path, install_path + '/OpenKit_Vendor');
log('Copied OpenKit_Vendor directory into install path.')

shutil.copytree(ok_resources_path, install_path + '/OpenKitResources');
log('Copied OpenKitResources directory into install path.')

project.add_folder(install_path + '/OpenKit_Vendor');
log('Added OpenKit_Vendor folder to Xcode project.')

project.add_folder(install_path + '/OpenKitResources');
log('Added OpenKitResources folder to Xcode project.')

project.saveFormat3_2()
log('Saved project.\n'
    '---------------------------')

