#include <UIKit/UIKit.h>

extern "C" void InitRemoteNotifications()
{
	[[UIApplication sharedApplication] registerForRemoteNotificationTypes:UIRemoteNotificationTypeAlert | UIRemoteNotificationTypeSound];
}

