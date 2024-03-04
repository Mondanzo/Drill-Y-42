using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUID = System.Guid;


[CustomEditor(typeof(RandomUniqueDecorationElement))]
public class RandomUniqueDecorationElementEditor : Editor {

	public override void OnInspectorGUI() {
		var uniqueDecoration = (RandomUniqueDecorationElement) target;
		if (uniqueDecoration.uniqueId == new GUID()) {
			if(GUILayout.Button("Create GUID")) {
				uniqueDecoration.GenerateUniqueID();
				serializedObject.ApplyModifiedProperties();
			}
		} else {
			EditorGUILayout.LabelField("UUID", uniqueDecoration.uniqueId.ToString());
		}
		DrawDefaultInspector();
	}
}