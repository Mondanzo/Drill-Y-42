#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(CaveGenerationSettings))]
public class CaveGenerationSettingsInspector : Editor {

	private struct MinMaxValue {
		public SerializedProperty minObject;
		public SerializedProperty maxObject;
		public float minEditor;
		public float maxEditor;
	}

	private MinMaxValue FindMinMax(string parameterName) {
		return new MinMaxValue {
			minObject = serializedObject.FindProperty("min" + parameterName),
			maxObject = serializedObject.FindProperty("max" + parameterName),
			minEditor = serializedObject.FindProperty("min" + parameterName).floatValue,
			maxEditor = serializedObject.FindProperty("max" + parameterName).floatValue
		};
	}

	private void CreateMinMaxSlider(ref MinMaxValue value, float minLimit, float maxLimit) {
		GUILayout.BeginHorizontal();
		value.minEditor = Mathf.Clamp(minLimit, EditorGUILayout.FloatField(value.minEditor, GUILayout.Width(60)), maxLimit);
		EditorGUILayout.MinMaxSlider(ref value.minEditor, ref value.maxEditor, minLimit, maxLimit);
		value.maxEditor = Mathf.Clamp(minLimit, EditorGUILayout.FloatField(value.maxEditor, GUILayout.Width(60)), maxLimit);
		GUILayout.EndHorizontal();
		value.minObject.floatValue = value.minEditor;
		value.maxObject.floatValue = value.maxEditor;
	}

	private MinMaxValue generationCurrency;
	private MinMaxValue rarity;
	private SerializedProperty danger;
	private MinMaxValue valueModifier;
	private SerializedProperty lootProbability;
	
	private SerializedProperty possibleSegments;
	private SerializedProperty oreRarities;
	private SerializedProperty biomeProbability;
	private SerializedProperty resourceProbability;
	private SerializedProperty curvatureSettings;
	private SerializedProperty collapsingWallImpact;

	private bool ShowResourceList = false;

	public void OnEnable() {
		generationCurrency = FindMinMax("GenerationCurrency");
		rarity = FindMinMax("Rarity");
		danger = serializedObject.FindProperty("Danger");
		valueModifier = FindMinMax("ValueModifier");
		lootProbability = serializedObject.FindProperty("LootProbability");
		possibleSegments = serializedObject.FindProperty("PossibleSegments");
		oreRarities = serializedObject.FindProperty("OreRarities");
		biomeProbability = serializedObject.FindProperty("BiomeProbability");
		resourceProbability = serializedObject.FindProperty("ResourceProbabilities");
		curvatureSettings = serializedObject.FindProperty("CurvatureSettings");
		collapsingWallImpact = serializedObject.FindProperty("CollapsingWallShakeImpact");
	}

	public override void OnInspectorGUI() {
		
		// Generation Currency UI
		GUILayout.Label("Generation Currency");
		CreateMinMaxSlider(ref generationCurrency, 1, 100);
		GUILayout.Label("Rarity");
		CreateMinMaxSlider(ref rarity, 0, 1);
		GUILayout.Label("Danger");
		EditorGUILayout.CurveField(danger, Color.green, Rect.MinMaxRect(0, 0, 1, 1));
		GUILayout.Label("Value Modifier");
		CreateMinMaxSlider(ref valueModifier, 1, 5);
		GUILayout.Label("Loot Value");
		EditorGUILayout.CurveField(lootProbability, Color.green, Rect.MinMaxRect(0, 0, 1, 1));
		GUILayout.Label("Curvature Settings (+1 0deg - -1 180deg)");
		EditorGUILayout.CurveField(curvatureSettings, Color.blue, Rect.MinMaxRect(0, -1, 1000, 1));
		EditorGUILayout.CurveField(collapsingWallImpact, Color.red, Rect.MinMaxRect(0, 0, 1000, 1));
		
		EditorGUILayout.PropertyField(oreRarities);
		EditorGUILayout.PropertyField(possibleSegments);
		
		GUILayout.Label("Biome Probabilities");
		BiomeList();
		
		ResourceList();

		serializedObject.ApplyModifiedProperties();

	}
	
	
	private void ResourceList() {

		ShowResourceList = EditorGUILayout.BeginFoldoutHeaderGroup(ShowResourceList, "Resource Probabilities");
		EditorGUILayout.EndFoldoutHeaderGroup();
		EditorGUI.indentLevel += 1;
		if(ShowResourceList) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"))) {
				resourceProbability.InsertArrayElementAtIndex(resourceProbability.arraySize);
				resourceProbability.GetArrayElementAtIndex(resourceProbability.arraySize - 1).FindPropertyRelative("Probability").animationCurveValue = AnimationCurve.Linear(0, 0, 1, 1);
				resourceProbability.GetArrayElementAtIndex(resourceProbability.arraySize - 1).FindPropertyRelative("Limits").boxedValue = new MinMaxInt(0, 6);
			}
	
			if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"))) {
				resourceProbability.DeleteArrayElementAtIndex(resourceProbability.arraySize - 1);
			}
			EditorGUILayout.EndHorizontal();
	
			var enumerator = resourceProbability.GetEnumerator();
			while(enumerator.MoveNext()) {
				var current = enumerator.Current as SerializedProperty;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(current.FindPropertyRelative("Type"), GUIContent.none);
				EditorGUILayout.CurveField(current.FindPropertyRelative("Probability"), Color.green, new Rect(0, 0, 1, 1), GUIContent.none);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(current.FindPropertyRelative("Limits"));
			}
		}
		
		EditorGUI.indentLevel -= 1;
	}
	
	
	private void BiomeList() {

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"))) {
			biomeProbability.InsertArrayElementAtIndex(biomeProbability.arraySize);
			biomeProbability.GetArrayElementAtIndex(biomeProbability.arraySize - 1).FindPropertyRelative("Probability").animationCurveValue = AnimationCurve.Linear(0, 0, 1, 1);
		}

		if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"))) {
			biomeProbability.DeleteArrayElementAtIndex(biomeProbability.arraySize - 1);
		}
		EditorGUILayout.EndHorizontal();

		var enumerator = biomeProbability.GetEnumerator();
		while(enumerator.MoveNext()) {
			var current = enumerator.Current as SerializedProperty;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.ObjectField(current.FindPropertyRelative("Biome"), GUIContent.none);
			EditorGUILayout.CurveField(current.FindPropertyRelative("Probability"), Color.green, new Rect(0, 0, 1, 1), GUIContent.none);
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif