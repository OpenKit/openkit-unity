using RestSharp;
using System;
using System.Threading;

namespace OpenKit
{

	public class OKMetadataResponse : OKIOResponse
	{
		public byte[] Raw { get; set; }
	}

	public class OKMetadataRequest
	{
		public static OKMetadataRequest Get(string url, OKRequestHandler<OKMetadataResponse> handler)
		{
			OKMetadataRequest request = new OKMetadataRequest(url);
			request.Get(handler);
			return request;
		}

		private Uri _uri;
		private RestClient _httpClient;
		private OKRequestHandler<OKMetadataResponse> _didFinish;
		private RestRequestAsyncHandle _killSwitch;

		protected OKMetadataRequest(string url)
		{
			_uri          = new Uri(url);
			_httpClient   = GetMetadataClient(_uri);
		}


		protected void Get(OKRequestHandler<OKMetadataResponse> handler)
		{
			_didFinish = handler;
			SynchronizationContext c = OKCtx.Ctx;
			if (c == null)
				DoGet(this);
			else
				c.Post(DoGet, this);
		}


		private void SynchronizedGet()
		{
			if (Cancelled) {
				// User cancelled before we made it to kick-off.
				var x = new OKMetadataResponse();
				x.Status = OKIOStatus.Cancelled;
				_didFinish(x);
				return;
			}

			var req = new RestRequest(_uri, Method.GET);

			_killSwitch = _httpClient.ExecuteAsync(req, response => {
				var y = new OKMetadataResponse();

				if (response.ResponseStatus == ResponseStatus.Completed)
					y.Raw = response.RawBytes;
				else if (response.ResponseStatus == ResponseStatus.Aborted)
					y.Status = OKIOStatus.Cancelled;
				else {
					y.Status = OKIOStatus.FailedWithError;
					y.Err = new OKException("Could not get metadata.  RestSharp response code: " + response.ResponseStatus);
				}

				SynchronizationContext c = OKCtx.Ctx;
				if (c == null)
					_didFinish(y);
				else
					c.Post(s => _didFinish(y), null);

				// We're all done.  Disable the kill switch:
				_killSwitch = null;
			});
		}


		public bool Cancelled { get; private set; }
		public void Cancel()
		{
			SynchronizationContext c = OKCtx.Ctx;
			if (c == null)
				DoCancel(null);
			else
				c.Post(DoCancel, null);
		}

		private void SynchronizedCancel()
		{
			if (!Cancelled) {
				Cancelled = true;
				if (_killSwitch != null) {
					System.Console.WriteLine("FLIPPING KILL SWITCH RIGHT MEOW.");
					_killSwitch.Abort();
					System.Console.WriteLine("FLIPPED.");
				}
			}
		}

		// This is a slight optimization. We'll reset it if we see the base url of the
		// metadata location change.  Otherwise we'll use the same client for each metadata request.
		private static RestClient __metadataClient;
		private static RestClient GetMetadataClient(Uri uri)
		{
			string baseUrl = uri.GetLeftPart(UriPartial.Authority);
			if (__metadataClient == null || !__metadataClient.BaseUrl.Equals(baseUrl)) {
				__metadataClient = new RestClient(baseUrl);
				__metadataClient.UseSynchronizationContext = false;
			}
			return __metadataClient;
		}

		private static void DoCancel(object state) { ((OKMetadataRequest)state).SynchronizedCancel(); }
		private static void DoGet(object state)    { ((OKMetadataRequest)state).SynchronizedGet();    }
	}
}
