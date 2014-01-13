using System;

namespace OpenKit
{
	public class OKCtx
	{
		public static System.Threading.SynchronizationContext Ctx;
		public static void SetCtx(System.Threading.SynchronizationContext c)
		{
			Ctx = c;
			if (Ctx != null) {
				OKLog.Info("SynchronizationContext is set.");
			}
		}
	}
}
