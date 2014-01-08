namespace OpenKit
{
	public delegate void OKRequestHandler<T>(T response);

	public enum OKIOStatus
	{
		Succeeded = 0,
		Cancelled = 1,
		FailedWithError = 2
	}

	public class OKIOResponse
	{
		public OKIOStatus Status { get; set; }
		public OKException Err { get; set; }
	}
}
