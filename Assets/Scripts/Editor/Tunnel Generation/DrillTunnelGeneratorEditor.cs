#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(DrillTunnelGenerator))]
public class DrillTunnelGeneratorEditor : Editor {
	private bool showGeneratorOptions;
	private int segmentsToGenerate = 5;

	private bool debugFoldout = false;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		var generator = (target as DrillTunnelGenerator);
		debugFoldout = EditorGUILayout.Foldout(debugFoldout, "Debug Info");
		if(debugFoldout) {
			EditorGUILayout.LabelField("Destructible Offset: " + generator.destructibleOffset);
			EditorGUILayout.LabelField("Collapsing Offset: " + generator.collapsableOffset);
			EditorGUILayout.LabelField("Total Current Length: " + generator.totalCurrentLength);
		}

		showGeneratorOptions = EditorGUILayout.Foldout(showGeneratorOptions, "Generator Options");

		if (showGeneratorOptions) {
			EditorGUILayout.PrefixLabel("Segments to generate");
			segmentsToGenerate = EditorGUILayout.IntSlider(segmentsToGenerate, 1, 20);

			if (GUILayout.Button("Generate")) {
				generator.Generate(segmentsToGenerate, true);
			}
		}
	}
}
#endif