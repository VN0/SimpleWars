using UnityEngine;

namespace Lean.Localization
{
	// This contains information about an alias for a language
	[System.Serializable]
	public class LeanCulture
	{
		// The language of this culture
		public string Language;

		// The list of all culture names for this language
		public string Alias;
	}
}