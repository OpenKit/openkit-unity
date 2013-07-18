#!/usr/bin/env perl
#
# Run this script after cloning the openkit-unity repo for the first time:
#   $ ./link.pl
#
# This script creates hardlinks between OKPlugins and Assets/Plugins of both
# Unity samples.  It does the same for OKEditor.  If you are having problems
# getting setup, please email Lou at lzell11@gmail.com.
#
# To unlink, use: 
#   $ ./link.pl -u
##

if ($ARGV[0] eq '-u') {
  unlink "Unity3Sample/Assets/Plugins" or warn "Could not unlink Plugins from unity3 sample: $!";
  unlink "Unity4Sample/Assets/Plugins" or warn "Could not unlink Plugins from unity4 sample: $!";
  unlink "Unity3Sample/Assets/Editor"  or warn "Could not unlink Editor from unity3 sample: $!";
  unlink "Unity4Sample/Assets/Editor"  or warn "Could not unlink Editor from unity4 sample: $!";
} 
else {
  link "OKPlugins", "Unity3Sample/Assets/Plugins" or warn "Could not link Plugins to unity3 sample: $!";
  link "OKPlugins", "Unity4Sample/Assets/Plugins" or warn "Could not link Plugins to unity4 sample: $!";
  link "OKEditor",  "Unity3Sample/Assets/Editor"  or warn "Could not link Editor to unity3 sample: $!";
  link "OKEditor",  "Unity4Sample/Assets/Editor"  or warn "Could not link Editor to unity4 sample: $!";
}
