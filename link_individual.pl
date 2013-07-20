#!/usr/bin/env perl
#
# This hardlinks in all the iOS only OpenKit assets.
#
# To unlink, use: 
#   $ ./link_individual.pl -u
##

$editor_dir  = "Assets/Editor";
$plugins_dir = "Assets/Plugins";
$ios_dir     = $plugins_dir . "/iOS";

@editor_refs  = qw(OpenKitPostprocessBuildPlayer.cs);
@plugins_refs = qw(RestSharp.dll OpenKit);
@ios_refs     = qw(libOpenKit.a libOpenKitUnity.a OpenKitResources OpenKit_Vendor);

if ($ARGV[0] eq '-u') {

  for $x (@editor_refs) {
    my $linkpath = $editor_dir."/".$x;
    if (-e $linkpath) {
      unlink $linkpath or warn "Could not unlink $x: $!";
    }
  }

  for $x (@plugins_refs) {
    my $linkpath = $plugins_dir."/".$x;
    if (-e $linkpath) {
      unlink $linkpath or warn "Could not unlink $x: $!";
    }
  }

  for $x (@ios_refs) {
    my $linkpath = $ios_dir."/".$x;
    if (-e $linkpath) {
      unlink $linkpath or warn "Could not unlink $x: $!";
    }
  }
} 
else {
  print "Note: Linking iOS related resources only!\n\n";

  mkdir $editor_dir  unless -d $editor_dir;
  mkdir $plugins_dir unless -d $plugins_dir;
  mkdir $ios_dir     unless -d $ios_dir;

  for $x (@editor_refs) {
    my $linkpath = $editor_dir."/".$x;
    if (!-e $linkpath) {
      link "../OK/openkit-unity/OKEditor/".$x, $linkpath or warn "Could not link $x: $!";
    }
  }

  for $x (@plugins_refs) {
    my $linkpath = $plugins_dir."/".$x;
    if (!-e $linkpath) {
      link "../OK/openkit-unity/OKPlugins/".$x, $linkpath or warn "Could not link $x: $!";
    }
  }

  for $x (@ios_refs) {
    my $linkpath = $ios_dir."/".$x;
    if (!-e $linkpath) {
      link "../OK/openkit-unity/OKPlugins/iOS/".$x, $linkpath or warn "Could not link $x: $!";
    }
  }
}
