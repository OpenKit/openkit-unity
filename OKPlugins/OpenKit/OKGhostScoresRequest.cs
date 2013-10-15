using System.Collections.Generic;

namespace OpenKit
{

	public class OKGhostScoresResponse : OKIOResponse
	{
		public List<OKScore> scores;
	}

	public class GhostScoresRequest
	{
		public          bool Cancelled   { get; private set; }
		public OKLeaderboard Leaderboard { get; private set; }

		private OKRequestHandler<OKGhostScoresResponse> _didFinish;
		private OKGhostScoresResponse _response;
		private List<OKScore> _pendingBufferLoad;

		#region Public API
		public GhostScoresRequest(OKLeaderboard leaderboard)
		{
			Leaderboard = leaderboard;
			_pendingBufferLoad = new List<OKScore>();
			_response = new OKGhostScoresResponse();
		}

		public void Get(OKRequestHandler<OKGhostScoresResponse> handler)
		{
			_didFinish = handler;
			OKFacebookUtilities.GetFacebookFriendsList(FriendsListDidLoad);   // No request handle for this.
		}

		public void Cancel()
		{
			Cancelled = true;
			foreach (OKScore score in _pendingBufferLoad) {
				score.CancelMetadataRequest();
			}
			_pendingBufferLoad.Clear();
			_response.Status = OKIOStatus.Cancelled;
			DoDidFinish();

		}
		#endregion

		private void DoDidFinish()
		{
			if (_didFinish != null) {
				_didFinish(_response);
			}
		}


		private void FriendsListDidLoad(List<string> ids, OKException e)
		{
			if (Cancelled)		// delegate protection
				return;

			if (e != null) {
				_response.Status = OKIOStatus.FailedWithError;
				_response.Err = new OKException("GhostScoresError: Failed to get FB Friends List.");
				DoDidFinish();
				return;
			}

			// No request handle:
			Leaderboard.GetFacebookFriendsScores(ids, FriendsScoresDidLoad);
		}


		private void FriendsScoresDidLoad(List<OKScore> scores, OKException e)
		{
			if (Cancelled)
				return;

			if (e != null) {
				_response.Status = OKIOStatus.FailedWithError;
				_response.Err = new OKException("GhostScoresError: Failed to get Social Scores.");
				DoDidFinish();
				return;
			}

			foreach (OKScore score in scores) {
				if (score.MetadataBuffer == null && score.MetadataLocation != null) {
					_pendingBufferLoad.Add(score);
					score.LoadMetadataBuffer(ScoreDidLoadMetadata);
				}
			}

			_response.scores = scores;
			if (_pendingBufferLoad.Count == 0) {
				DoDidFinish();
			}
		}


		private void ScoreDidLoadMetadata(OKScore score)
		{
			if (Cancelled)
				return;

			_pendingBufferLoad.Remove(score);
			if(_pendingBufferLoad.Count == 0) {
				DoDidFinish();
			}
		}
	}
}