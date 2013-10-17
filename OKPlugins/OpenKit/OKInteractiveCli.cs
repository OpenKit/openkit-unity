// Note that the path addition of '../' is because of the RestSharp location.
//
// $ gmcs -debug -define:INTERACTIVE OKInteractiveCli.cs OKServiceRequest.cs OKIOResponse.cs JSONObject.cs JSONObjectExt.cs OKException.cs OKUploadBuffer.cs OKLog.cs OKCtx.cs OKFormPair.cs -r:../RestSharp.dll
// $ MONO_PATH='../' mono  --debug OKInteractiveCli.exe


#if INTERACTIVE
using System;
using OpenKit;
using RestSharp;
using System.Threading;

namespace OpenKit
{
	public static class InteractiveCli
	{

		static public void Main()
		{
			Console.WriteLine(CommandHelp);
			while (true) {
				Console.Write(">> ");
				string line = Console.ReadLine();
				switch (line) {
				case "l" :
					OKServiceRequest.Get("/leaderboards", null, response => {
						OKLog.Info("Response Json Object: " + response.Obj);
					});
					break;

				case "h" :
					Console.WriteLine(CommandHelp);
					break;

				case "sh" :
					Console.Write("Enter the new host (e.g. api.openkit.io): ");
					string h = Console.ReadLine();
					var u = new Uri(String.Format("http://{0}", h));
					OKServiceRequest.Host = u.Host;
					OKServiceRequest.Port = u.Port;
					break;

				case "sa" :
					Console.Write("Enter the new app key: ");
					string a = Console.ReadLine();
					OKServiceRequest.AppKey = a;
					break;

				case "ss" :
					Console.Write("Enter the new secret key: ");
					string s = Console.ReadLine();
					OKServiceRequest.SecretKey = s;
					break;

				case "p" :
					Console.WriteLine("Not implemented");
					break;
				}
			}
//			System.Threading.Thread.Sleep(Timeout.Infinite);
		}


public const string CommandHelp = @"
Commands:
sh - Set Host
sa - Set App Key
ss - Set Secret Key
l - Show Leaderboards
p - Post a Score
h - Show help
";
	}
}

#endif
