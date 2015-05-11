using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NobleMuffins.SoundtrackPlus {

	public class MusicPlaybackController : MonoBehaviour
	{
		private static MusicPlaybackController Instance;

		public float fadeTime = 1f;

		private readonly ICollection<AudioSource> audioSources = new HashSet<AudioSource>();
		private readonly ICollection<AudioSource> audioSourceRecycleBin = new HashSet<AudioSource>();

		private MusicTrackConfiguration currentTrack;
		private MusicTrackConfiguration NullTrackConfiguration;

		void Awake() {
			if(Instance == null) {
				Instance = this;
				GameObject.DontDestroyOnLoad(gameObject);
			}
			else if(Instance != this) {
				GameObject.DestroyImmediate(this);
			}
		}

		void Start() {
			NullTrackConfiguration = ScriptableObject.CreateInstance<MusicTrackConfiguration>();
			NullTrackConfiguration.clip = null;
			NullTrackConfiguration.transitionType = ETransition.FadePriorAndStart;
		}

		void Update() {
			var filter = AbstractMusicFilter.Instance;

			var requestedTrack = filter == null ? null : filter.CurrentMusicTrack;

			currentTrack = requestedTrack ?? NullTrackConfiguration;

			var isCurrentTrackRepresented = audioSources.Any((source) => source.clip == currentTrack.clip);
			var isCurrentAlone = isCurrentTrackRepresented && audioSources.Count == 1;
			var isAnythingPlaying = audioSources.Any((source) => source.isPlaying);

			if(!isCurrentTrackRepresented && currentTrack.clip != null) {
				var source = gameObject.AddComponent<AudioSource>();

				source.clip = currentTrack.clip;
				source.loop = true;

				if(!isAnythingPlaying) {
					source.volume = 1f;
					source.playOnAwake = true;
				}
				else switch(currentTrack.transitionType)
				{
				case ETransition.FadePriorAndStart:
					source.volume = 0f;
					break;
				case ETransition.Fade:
					source.volume = 0f;
					source.Play();
					break;
				case ETransition.HaltPriorAndStart:
					source.volume = 1f;
					source.Play();
					break;
				}

				audioSources.Add(source);
			}

			var haltAllPrior = currentTrack.transitionType == ETransition.HaltPriorAndStart;

			foreach(var source in audioSources)
			{
				var isCurrent = source.clip == currentTrack.clip;

				if(isCurrent && !source.isPlaying && isCurrentAlone)
				{
					source.Play();
				}

				if(isCurrent && source.volume < 1f) {
					source.volume = Mathf.Min(1f, source.volume + Time.deltaTime / fadeTime);
				}
				else if(!isCurrent && source.volume > 0f) {
					source.volume = Mathf.Max(0f, source.volume - Time.deltaTime / fadeTime);
				}

				var dropNow = !isCurrent && (haltAllPrior || source.volume == 0f);

				if(dropNow) {
					audioSourceRecycleBin.Add(source);
				}
			}

			foreach(var source in audioSourceRecycleBin)
			{
				audioSources.Remove(source);
				source.Stop();
				GameObject.DestroyImmediate(source);
			}

			audioSourceRecycleBin.Clear();
		}
	}
}
