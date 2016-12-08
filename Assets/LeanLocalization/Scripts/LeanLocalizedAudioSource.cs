using UnityEngine;
using UnityEngine.UI;

namespace Lean.Localization
{
	// This component will update an AudioSource component with localized text, or use a fallback if none is found
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AudioSource))]
	public class LeanLocalizedAudioSource : LeanLocalizedBehaviour
	{
		[Tooltip("If PhraseName couldn't be found, this clip will be used")]
		public AudioClip FallbackAudioClip;
		
		// This gets called every time the translation needs updating
		public override void UpdateTranslation(LeanTranslation translation)
		{
			// Get the AudioSource component attached to this GameObject
			var audioSource = GetComponent<AudioSource>();
			
			// Use translation?
			if (translation != null)
			{
				audioSource.clip = translation.Object as AudioClip;
			}
			// Use fallback?
			else
			{
				audioSource.clip = FallbackAudioClip;
			}
		}

		protected virtual void Awake()
		{
			// Should we set FallbackAudioClip?
			if (FallbackAudioClip == null)
			{
				// Get the AudioSource component attached to this GameObject
				var audioSource = GetComponent<AudioSource>();
				
				// Copy current sprite to fallback
				FallbackAudioClip = audioSource.clip;
			}
		}
	}
}