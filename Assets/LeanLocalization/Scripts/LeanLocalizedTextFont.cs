using UnityEngine;
using UnityEngine.UI;

namespace Lean.Localization
{
	// This component will update a Text component's Font with a localized font, or use a fallback if none is found
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class LeanLocalizedTextFont : LeanLocalizedBehaviour
	{
		[Tooltip("If PhraseName couldn't be found, this font will be used")]
		public Font FallbackFont;
		
		// This gets called every time the translation needs updating
		public override void UpdateTranslation(LeanTranslation translation)
		{
			// Get the Text component attached to this GameObject
			var text = GetComponent<Text>();
			
			// Use translation?
			if (translation != null)
			{
				text.font = translation.Object as Font;
			}
			// Use fallback?
			else
			{
				text.font = FallbackFont;
			}
		}

		protected virtual void Awake()
		{
			// Should we set FallbackFont?
			if (FallbackFont == null)
			{
				// Get the Text component attached to this GameObject
				var text = GetComponent<Text>();
				
				// Copy current font to fallback
				FallbackFont = text.font;
			}
		}
	}
}