using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NobleMuffins.SoundtrackPlus
{
	public abstract class AbstractMusicFilter : MonoBehaviour
	{
		private static readonly List<AbstractMusicFilter> OrderedFilters = new List<AbstractMusicFilter>();

		public int priority = 0;
			
		public static AbstractMusicFilter Instance {
			get {
				AbstractMusicFilter filter = null;
				var orderedFilters = OrderedFilters.ToArray();
				for(int i = 0; i < orderedFilters.Length && filter == null; i++)
				{
					var tentative = orderedFilters[i];
					if(tentative.CurrentMusicTrack != null)
					{
						filter = tentative;
					}
				}
				return filter;
			}
		}

		public abstract MusicTrackConfiguration CurrentMusicTrack { get; }

		void OnEnable() {
			if(OrderedFilters.Contains(this) == false) {
				OrderedFilters.Add(this);
				OrderedFilters.Sort((alfa, bravo) => alfa.priority - bravo.priority);
			}
		}
		
		void OnDisable() {
			if(OrderedFilters.Contains(this))
			{
				OrderedFilters.Remove(this);
			}
		}
	}
}