using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnityEditor.ProjectStripper
{

	public class Session : ScriptableObject
	{
		[Serializable]
		public class StrippingStep
		{
			public string rootPath;
			public List<string> files = new List<string> ();
			public StrippingOperationType operation;
		}

		[SerializeField] private string _backupAreaRoot;
		[SerializeField] private List<StrippingStep> _steps;
		private List<StrippingStep> _shadowSteps;

		private StrippingStep _activeStep;

		public const string DefaultAssetPath = "Assets/Project Stripping Session.asset";

		private static Session _defaultSession;

		public static Session DefaultSession {
			get {
				if (!_defaultSession) {
					var existingSessionPath = AssetDatabase.FindAssets ("t:" + typeof(Session).Name);
					if (existingSessionPath.Length == 0) {
						_defaultSession = CreateInstance<Session> ();
						AssetDatabase.CreateAsset (_defaultSession, DefaultAssetPath);
					} else {
						_defaultSession = AssetDatabase.LoadAssetAtPath<Session> (AssetDatabase.GUIDToAssetPath (existingSessionPath [0]));
					}
				}
				return _defaultSession;
			}
		}

		public void Reset ()
		{
			// Generate a temporary path for moving assets to - outside of the project root, so it is
			// definitely not going to be included if the user 
			_backupAreaRoot = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
		}

		public void OnEnable()
		{
			if (_steps == null) _steps = new List<StrippingStep>();
			_shadowSteps = new List<StrippingStep>(_steps);
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		public void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedo;
		}

		private void OnUndoRedo()
		{
			if (_steps.Count == _shadowSteps.Count) return;

			try
			{
				AssetDatabase.StartAssetEditing();

				while (_steps.Count > _shadowSteps.Count)
				{
					var step = _steps[_shadowSteps.Count];
					Execute(step);
					_shadowSteps.Add(step);
				}
	
				while (_steps.Count < _shadowSteps.Count)
				{
					var step = _shadowSteps[_shadowSteps.Count - 1];
					Unexecute(step);
					_shadowSteps.RemoveAt(_shadowSteps.Count - 1);
				}
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
				AssetDatabase.Refresh();

				EditorApplication.delayCall += DeferredWindowUpdate;
			}
		}

		private void DeferredWindowUpdate()
		{
			foreach(SessionWindow wnd in FindObjectsOfType<SessionWindow>())
					wnd.Repaint();
			EditorApplication.delayCall -= DeferredWindowUpdate;
		}

		private void CollectWorkFromPath (List<string> workList, string path)
		{
			if (path.EndsWith (".meta"))
				return;
			if (!System.IO.Directory.Exists (path) && !System.IO.File.Exists (path))
				throw new System.IO.FileNotFoundException ("Unable to find the path \"" + path + "\".", path);

			if (System.IO.Directory.Exists (path)) {
				// Collect work recursively
				foreach (var child in System.IO.Directory.GetFiles(path))
					CollectWorkFromPath (workList, child);
				// also from subdirectories
				foreach (var child in System.IO.Directory.GetDirectories(path))
					CollectWorkFromPath (workList, child);
			}

			if (File.Exists (path)) // Don't add directories themselves onto the list
			workList.Add (path);
			if (File.Exists (path + ".meta")) // However, *do* add their .meta files
			workList.Add (path + ".meta");
		}

		private void CollectWorkByAssetType (List<string> workList, string path, string typeName)
		{
			foreach (var guid in AssetDatabase.FindAssets("t:" + typeName, new[]{path}))
				CollectWorkFromPath (workList, AssetDatabase.GUIDToAssetPath (guid));
		}

		private void CollectWorkByAssetType<T> (List<string> workList, string path) where T : UnityEngine.Object
		{
			CollectWorkByAssetType(workList, path, typeof(T).Name);
		}

		private void CollectVisibleRenderersToCamera (Camera c, HashSet<Renderer> renderers)
		{
			// Force a render so that visibility information is updated
			c.Render ();
			
			var visibleRenderers = FindObjectsOfType<Renderer> ().Where (r => r.isVisible);
			foreach (var r in visibleRenderers)
				renderers.Add (r);
		}

		static void RemovePathsAndMetas(List<string> files, HashSet<string> paths)
		{
			foreach(var p in paths)
				files.RemoveAll(f => f == p || f == (p + ".meta"));
		}

		static void CollectPathsForObjects<T> (IEnumerable<T> objects, HashSet<string> paths) where T : UnityEngine.Object
		{
			foreach (var path in objects.Select(o => AssetDatabase.GetAssetPath(o)).Where(p => !string.IsNullOrEmpty(p)))
				paths.Add(path);
		}

		static void CollectPathForObject (UnityEngine.Object obj, HashSet<string> paths)
		{
			var path = AssetDatabase.GetAssetPath(obj);
			if (!string.IsNullOrEmpty(path)) paths.Add(path);
		}

		static HashSet<Texture> CollectTexturesFromMaterial (Material mat)
		{
			var result = new HashSet<Texture>();
			var so = new SerializedObject (mat);
			var texturesProp = so.FindProperty ("m_SavedProperties.m_TexEnvs");
			for (int i = 0; i < texturesProp.arraySize; ++i) {
				var tex = texturesProp.GetArrayElementAtIndex (i).FindPropertyRelative ("second.m_Texture").objectReferenceValue as Texture;
				if (tex)
					result.Add (tex);
			}
			so.Dispose ();
			return result;
		}

		static void CollectAssetsUsedByRenderers (IEnumerable<Renderer> renderers, HashSet<string> assetPaths)
		{
			foreach (var r in renderers) {
				var mf = r.GetComponent<MeshFilter> ();
				if (mf && mf.sharedMesh)
					CollectPathForObject (mf.sharedMesh, assetPaths);
				foreach (var mat in r.sharedMaterials) {
					if (!mat)
						continue;
					CollectPathForObject (mat, assetPaths);
					CollectPathForObject (mat.shader, assetPaths);
					CollectPathsForObjects (CollectTexturesFromMaterial (mat), assetPaths);
				}
				// Keep collision meshes, too
				var mc = r.GetComponent<MeshCollider> ();
				if (mc && mc.sharedMesh)
					CollectPathForObject (mc.sharedMesh, assetPaths);
			}
		}

		private StrippingStep BuildStrippingStep (string path, StrippingOperationType operation)
		{
			var result = new StrippingStep ();
			result.rootPath = path;
			result.operation = operation;

			switch (operation) {
			case StrippingOperationType.StripAll:
				{
					CollectWorkFromPath (result.files, path);
					break;
				}
			case StrippingOperationType.StripTextures:
				{
					CollectWorkByAssetType<Texture> (result.files, path);
					break;
				}
			case StrippingOperationType.StripModels:
				{
					CollectWorkByAssetType(result.files, path, "Model");
					break;
				}
			case StrippingOperationType.StripMaterials:
				{
					CollectWorkByAssetType<Material> (result.files, path);
					break;
				}
			case StrippingOperationType.StripAudio:
				{
					CollectWorkByAssetType<AudioClip> (result.files, path);
					break;
				}
			case StrippingOperationType.StripShaders:
				{
					CollectWorkByAssetType<Shader> (result.files, path);
					break;
				}
			case StrippingOperationType.StripArtNotInSceneView:
				{
					CollectWorkByAssetType<Texture> (result.files, path);
					CollectWorkByAssetType(result.files, path, "Model");
					CollectWorkByAssetType<Material> (result.files, path);
					CollectWorkByAssetType<Shader> (result.files, path);

					HashSet<Renderer> visibleRenderers = new HashSet<Renderer> ();
					foreach (var camera in SceneView.GetAllSceneCameras())
						CollectVisibleRenderersToCamera (camera, visibleRenderers);

					var exclusionList = new HashSet<string>();

					CollectAssetsUsedByRenderers (visibleRenderers, exclusionList);

					// Also collect cookies from any lights in the scene, and cubemaps from any reflection probes
					CollectPathsForObjects(FindObjectsOfType<Light>()
										  .Where(l => l.enabled && l.gameObject.activeInHierarchy && l.cookie)
										  .Select(l => l.cookie), exclusionList);
					
					foreach (var p in FindObjectsOfType<ReflectionProbe>())
					{
						CollectPathForObject(p.bakedTexture, exclusionList);
						CollectPathForObject(p.customBakedTexture, exclusionList);
					}

					RemovePathsAndMetas(result.files, exclusionList);

					break;
				}
			case StrippingOperationType.StripArtNotInLoadedScenes:
				{
					CollectWorkByAssetType<Texture> (result.files, path);
					CollectWorkByAssetType(result.files, path, "Model");
					CollectWorkByAssetType<Material> (result.files, path);
					CollectWorkByAssetType<Shader> (result.files, path);

					var scenePaths = UnityEditor.SceneManagement.EditorSceneManager.GetAllScenes().Where(s => s.isLoaded).Select(s => s.path).ToArray();
					var assets = new HashSet<string>(AssetDatabase.GetDependencies(scenePaths, true));
					
					RemovePathsAndMetas(result.files, assets);
					break;
				}
			default:
				throw new ArgumentOutOfRangeException ("operation");
			}

			return result;
		}

		private void Execute (StrippingStep step)
		{
			// First, verify...
			foreach (var srcPath in step.files) {
				if (!File.Exists (srcPath) && !Directory.Exists (srcPath))
					throw new InvalidOperationException ("The file or directory at path " + srcPath + " is in the worklist but missing on disk.");
			}

			// Now execute
			foreach (var srcPath in step.files) {
				var targetPath = Path.Combine (_backupAreaRoot, srcPath);
				var parentPath = Path.GetDirectoryName (targetPath);
				Directory.CreateDirectory (parentPath);
				FileUtil.MoveFileOrDirectory (srcPath, targetPath);

				// Remove moved directories
				if (srcPath.EndsWith (".meta")) {
					var dataPath = AssetDatabase.GetAssetPathFromTextMetaFilePath (srcPath);
					if (Directory.Exists (dataPath))
						Directory.Delete (dataPath);
				}
			}
		}

		private void Unexecute (StrippingStep step)
		{
			foreach (var srcPath in step.files) {
				var targetPath = Path.Combine (_backupAreaRoot, srcPath);
				var parentPath = Path.GetDirectoryName (srcPath);
				Directory.CreateDirectory (parentPath);
				FileUtil.MoveFileOrDirectory (targetPath, srcPath);

				// Deal with empty directories
				if (srcPath.EndsWith (".meta")) {
					var dataPath = AssetDatabase.GetAssetPathFromTextMetaFilePath (srcPath);
					if (!File.Exists (dataPath) && !Directory.Exists (dataPath))
						Directory.CreateDirectory (dataPath);
				}
			}
		}

		public void Strip (string path, StrippingOperationType op)
		{
			Undo.RecordObject(this, "Strip Assets");
			var step = BuildStrippingStep (path, op);
			Execute (step);

			_steps.Add (step);
			_shadowSteps.Add (step);
		}

		public void UndoLastOperation ()
		{
			if (_steps.Count == 0)
				throw new InvalidOperationException ();

			Undo.RecordObject(this, "Undo Stripping");

			var step = _steps.Last ();
			Unexecute (step);
			_steps.RemoveAt (_steps.Count - 1);
			_shadowSteps.RemoveAt (_shadowSteps.Count - 1);
		}
	}
}