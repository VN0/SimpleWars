using UnityEngine;
using UnityEngine.UI;

namespace Lean.Localization
{
	// This component will update a Text component with localized text, or use a fallback if none is found, and format the string with custom arguments
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class LeanLocalizedTextArgs : LeanLocalizedBehaviour
	{
		[Tooltip("If PhraseName couldn't be found, this text will be used")]
		public string FallbackText;

		[System.NonSerialized]
		public object[] Args;
		
		public void SetArg(object arg, int index)
		{
			if (index >= 0)
			{
				var update = false;

				if (Args == null)
				{
					Args = new object[index + 1]; update = true;
				}
				else if (index >= Args.Length)
				{
					System.Array.Resize(ref Args, index + 1); update = true;
				}

				if (Args[index] != arg)
				{
					Args[index] = arg; update = true;
				}

				if (update == true)
				{
					UpdateLocalization();
				}
			}
		}

		// This gets called every time the translation needs updating
		public override void UpdateTranslation(LeanTranslation translation)
		{
			// Get the Text component attached to this GameObject
			var text = GetComponent<Text>();
			
			// Use translation?
			if (translation != null)
			{
				if (Args != null)
				{
					text.text = string.Format(translation.Text, Args);
				}
				else
				{
					text.text = translation.Text;
				}
			}
			// Use fallback?
			else
			{
				if (Args != null)
				{
					text.text = string.Format(FallbackText, Args);
				}
				else
				{
					text.text = FallbackText;
				}
			}
		}

		protected virtual void Awake()
		{
			// Should we set FallbackText?
			if (string.IsNullOrEmpty(FallbackText) == true)
			{
				// Get the Text component attached to this GameObject
				var text = GetComponent<Text>();
				
				// Copy current text to fallback
				FallbackText = text.text;
			}
		}
	}
}