using UnityEngine;
using System.Collections;

namespace NobleMuffins.SoundtrackPlus {

	public enum ETransition {
		FadePriorAndStart, Fade, HaltPriorAndStart
	}

	public class MusicTrackConfiguration : ScriptableObject {
		public AudioClip clip;
		public ETransition transitionType = ETransition.FadePriorAndStart;
	}

}