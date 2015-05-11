using UnityEngine;
using System.Collections;

namespace NobleMuffins.SoundtrackPlus {
	public class SingleTrackMusicFilter : AbstractMusicFilter {

		public MusicTrackConfiguration currentMusicTrack;

		public override MusicTrackConfiguration CurrentMusicTrack {
			get {
				return currentMusicTrack;
			}
		}
	}
}