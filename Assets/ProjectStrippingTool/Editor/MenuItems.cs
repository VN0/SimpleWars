using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.ProjectStripper
{

	public static class MenuItems
	{
		public const string BaseMenuName = "Assets/Project Stripping/";
		public const string StripSelectedDirectoriesName = BaseMenuName + "Strip all assets in selection";
		public const string StripSelectedTexturesName = BaseMenuName + "Strip all textures";
		public const string StripSelectedModelsName = BaseMenuName + "Strip all models";
		public const string StripSelectedMaterialsName = BaseMenuName + "Strip all materials";
		public const string StripSelectedAudioName = BaseMenuName + "Strip all audio clips";
		public const string StripArtNotInViewName = BaseMenuName + "Strip art not in scene view";
		public const string StripArtNotInLoadedScenesName = BaseMenuName + "Strip art not used by loaded scenes";

		private static IEnumerable<string> GetAssetPathsFromSelection ()
		{
			return Selection.assetGUIDs.Select (g => AssetDatabase.GUIDToAssetPath (g));
		}

		[MenuItem (StripSelectedDirectoriesName, true)]
		[MenuItem (StripSelectedTexturesName, true)]
		[MenuItem (StripSelectedModelsName, true)]
		[MenuItem (StripSelectedMaterialsName, true)]
		[MenuItem (StripSelectedAudioName, true)]
		[MenuItem (StripArtNotInViewName, true)]
		[MenuItem (StripArtNotInLoadedScenesName, true)]
		public static bool IsAnyAssetPathsSelected ()
		{
			return GetAssetPathsFromSelection ().Any ();
		}

		private static void StripSelected (StrippingOperationType operation)
		{
			try {
				AssetDatabase.StartAssetEditing ();
				foreach (var path in GetAssetPathsFromSelection())
					Session.DefaultSession.Strip (path, operation);
			} finally {
				AssetDatabase.StopAssetEditing ();
				AssetDatabase.Refresh ();
			}
		}

		[MenuItem (StripSelectedDirectoriesName, false, 1)]
		public static void StripSelectedDirectories ()
		{ 
			StripSelected (StrippingOperationType.StripAll); 
		}

		[MenuItem (StripSelectedTexturesName, false, 101)]
		public static void StripSelectedTextures ()
		{
			StripSelected (StrippingOperationType.StripTextures);
		}

		[MenuItem (StripSelectedModelsName, false, 102)]
		public static void StripSelectedModels ()
		{
			StripSelected (StrippingOperationType.StripModels);
		}

		[MenuItem (StripSelectedMaterialsName, false, 103)]
		public static void StripSelectedMaterials ()
		{
			StripSelected (StrippingOperationType.StripMaterials);
		}

		[MenuItem (StripSelectedAudioName, false, 104)]
		public static void StripSelectedAudio ()
		{
			StripSelected (StrippingOperationType.StripAudio);
		}

		[MenuItem (StripArtNotInViewName, false, 501)]
		public static void StripArtNotInView ()
		{
			StripSelected (StrippingOperationType.StripArtNotInSceneView);
		}

		[MenuItem (StripArtNotInLoadedScenesName, false, 502)]
		public static void StripArtNotInLoadedScenes ()
		{
			StripSelected (StrippingOperationType.StripArtNotInLoadedScenes);
		}
	}
}