using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

// Tail with this to see output clearly:
// 
//   tail -f ~/Library/Logs/Unity/Editor.log  | grep ^OK --line-buffered | awk '{ printf("%-9s %-60s", $1, $2); $1=$2=""; printf("%s\n", $0); }'

namespace OpenKit {
	public class OKLog {
		
		// [Conditional("DEBUG")]
		public static void Info(string msg) {
			System.Console.WriteLine("OK {0} {1}", StackInfo(), msg);
		}
		
		public static void Error(string msg) {
			System.Console.WriteLine("OKERROR {0} {1}", StackInfo(), msg);
		}
		
		protected static string StackInfo() {
			StackTrace t = new System.Diagnostics.StackTrace(true);
			StackFrame sf = t.GetFrame(2);
			if (sf != null) {
				string filename = sf.GetFileName();
				if (filename != null) {
					filename = Regex.Replace(filename, @".*?Assets\/(.*)", "$1");
					int line = sf.GetFileLineNumber();
					return filename + ":" + line;
				} 
			}
			return "";
		}
	}
}
