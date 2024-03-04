using System;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveGenerator))]
public class CaveGeneratorEditor : CaveSegmentEditor {

	private SerializedProperty generationSettings;
	
	private GameObject generatedCave;

	public new void OnEnable() {
		generationSettings = serializedObject.FindProperty("generationSettings");
		base.OnEnable();
	}

	public void OnDisable() {
		if (generatedCave) DestroyImmediate(generatedCave);
	}

	public override void OnInspectorGUI() {
		GUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(generationSettings);
		if (GUILayout.Button("New", GUILayout.Width(60))) {
			var settings = ScriptableObject.CreateInstance<CaveGenerationSettings>();

			try {
				AssetDatabase.StartAssetEditing();
				var path = AssetDatabase.GenerateUniqueAssetPath("Assets/Cave Generation Settings.asset");
				AssetDatabase.CreateAsset(settings, path);
			} finally {
				AssetDatabase.StopAssetEditing();
			}
			generationSettings.objectReferenceValue = settings;
			serializedObject.ApplyModifiedProperties();
		}
		GUILayout.EndHorizontal();
		base.OnInspectorGUI();
		
		if (GUILayout.Button(generatedCave == null ? "Generate" : "Regenerate")) {
			if(generatedCave) DestroyImmediate(generatedCave);
			Generate();
		}
		if(generatedCave != null) {
			if (GUILayout.Button("Clear")) {
				DestroyImmediate(generatedCave);
			}
		}
	}
	
	public void Generate() {
		var generator = (CaveGenerator) target;

		var go = new GameObject("Generated Cave");

		go.AddComponent<CaveGeneratorHelper>().SetHolder(generator.gameObject);
		
		go.transform.position = generator.transform.position;
		go.transform.rotation = generator.transform.rotation;

		var instance = Instantiate(generator, go.transform);
		instance.transform.localPosition = Vector3.zero;
		instance.transform.localRotation = Quaternion.identity;
		
		foreach (var collider in generator.GetComponentsInChildren<Collider>()) {
			collider.enabled = false;
		}
		
		EditorCoroutineUtility.StartCoroutine(instance.Generate(), this);
		
		SceneVisibilityManager.instance.Hide(generator.gameObject, true);
		generatedCave = go;
	}
}

[ExecuteInEditMode]
public class CaveGeneratorHelper : MonoBehaviour {

	private GameObject holder;
	
	public void SetHolder(GameObject holder) {
		this.holder = holder;
	}
	
	
	public void OnDestroy() {
		if (!EditorApplication.isPlaying) {
			if (holder != null) {
				SceneVisibilityManager.instance.Show(holder, true);
				foreach (var collider in holder.GetComponentsInChildren<Collider>()) {
					collider.enabled = true;
				}
			}
			
		}
	}
}