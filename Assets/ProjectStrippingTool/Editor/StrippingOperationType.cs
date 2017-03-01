using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnityEditor.ProjectStripper
{
	public enum StrippingOperationType
	{
		StripAll,
		StripTextures,
		StripModels,
		StripMaterials,
		StripAudio,
		StripShaders,
		StripArtNotInSceneView,
		StripArtNotInLoadedScenes
	}
	
}