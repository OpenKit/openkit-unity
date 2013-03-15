using System;

namespace OpenKit 
{
	public class OKCloudException : Exception
	{
		public OKCloudException(string message) : base(message) {}
	}
}