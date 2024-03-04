using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveSegment))]
public class CaveSegmentEditor : Editor {
	
	private SerializedProperty debugPointList;
	private SerializedProperty segmentCurrency;
	private SerializedProperty rarity;
	private SerializedProperty debugAABB;

	private bool debugPointListFoldedOut;

	
	// Link Point Wizard
	private bool newLinkPointWizardActive = false;
	private Vector3 firstPoint;
	private Vector3 secondPoint;
	private bool flipDirection = false;

	public void OnEnable() {
		debugPointList = serializedObject.FindProperty("linkPoints");
		segmentCurrency = serializedObject.FindProperty("SegmentValue");
		rarity = serializedObject.FindProperty("Rarity");
		debugAABB = serializedObject.FindProperty("showDebugBoundingBoxes");
		
	}

	public override void OnInspectorGUI() {
		var segment = (CaveSegment) target;

		bool infoOnly = segment.GetComponentInParent<CaveGeneratorHelper>();

		if (infoOnly) {
			GUILayout.Label("Do not edit.");
		}

		EditorGUILayout.PropertyField(debugAABB);

		if (!infoOnly) {
			EditorGUILayout.PropertyField(segmentCurrency);
			EditorGUILayout.PropertyField(rarity);
			serializedObject.ApplyModifiedProperties();
		}
		
		if(!infoOnly) {
			if(!newLinkPointWizardActive) {
				if (GUILayout.Button("LinkPoint Wizard")) {
					newLinkPointWizardActive = true;
					firstPoint = segment.transform.position;
					secondPoint = segment.transform.position;
				}
			}
			else {
				if (GUILayout.Button("Cancel LinkPoint Wizard")) {
					newLinkPointWizardActive = false;
					firstPoint = Vector3.zero;
					secondPoint = Vector3.zero;
				}

				if (firstPoint == secondPoint) {
					GUILayout.Label("Points are on the same position! Can't create new point.");
				}
				else {
					// Points differ
					if (GUILayout.Button("Flip Direction")) {
						flipDirection = !flipDirection;
					}

					if (GUILayout.Button("Create LinkPoint")) {
						var centerPoint = secondPoint - firstPoint;
						var linkPoint = new LinkPoint { position = Vector3.zero, rotation = Quaternion.identity, debugColor = Random.ColorHSV(0, 1, 0.5f, 1, 0.8f, 1f) };
						linkPoint.position = firstPoint + centerPoint / 2;
						linkPoint.rotation = Quaternion.FromToRotation(Vector3.forward, Quaternion.Euler(0, flipDirection ? 90 : -90, 0) * centerPoint.normalized);
						Undo.RecordObject(target, "Added new link point");
						segment.linkPoints.Add(linkPoint);
						newLinkPointWizardActive = false;
					}
				}
			}
		}
		
		GUILayout.BeginHorizontal();

		if(!infoOnly) {
			if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"))) {
				Undo.RecordObject(target, "Update Link Points");
				segment.linkPoints.Add(new LinkPoint {position = Vector3.zero, rotation = Quaternion.identity, debugColor = Random.ColorHSV(0, 1, 0.5f, 1, 0.8f, 1f)});
				EditorWindow.GetWindow<SceneView>().Repaint();
			}
		}
		
		GUILayout.Label($"{segment.linkPoints.Count} points");

		if(!infoOnly) {
			if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"))) {
				Undo.RecordObject(target, "Update Link Points");
				if(segment.linkPoints.Count > 0) segment.linkPoints.RemoveAt(segment.linkPoints.Count -1 );
				EditorWindow.GetWindow<SceneView>().Repaint();
			}
		}

		GUILayout.EndHorizontal();
		if(!infoOnly) serializedObject.ApplyModifiedProperties();
		debugPointListFoldedOut = EditorGUILayout.Foldout(debugPointListFoldedOut, "Show Debug Points");
		EditorGUI.indentLevel++;
		if (debugPointListFoldedOut) {
			var enumerator = debugPointList.GetEnumerator();
			while(enumerator.MoveNext()) {
				var property = enumerator.Current as SerializedProperty;
				EditorGUILayout.PropertyField(property);
			}
		}
		EditorGUI.indentLevel--;
	}

	public void OnSceneGUI() {
		var segment = (CaveSegment) target;

		if (newLinkPointWizardActive) {
			Handles.matrix = segment.transform.localToWorldMatrix;
			Handles.color = Color.red;
			HandleUtility.FindNearestVertex(HandleUtility.WorldToGUIPoint(Handles.FreeMoveHandle(firstPoint, 0.1f, Vector3.one * 0.1f, Handles.DotHandleCap)), out firstPoint);
			
			Handles.color = Color.blue;
			HandleUtility.FindNearestVertex(HandleUtility.WorldToGUIPoint(Handles.FreeMoveHandle(secondPoint, 0.1f, Vector3.one * 0.1f, Handles.DotHandleCap)), out secondPoint);
			
			Handles.DrawDottedLine(firstPoint, secondPoint, 1f);
			var centerPoint = secondPoint - firstPoint;
			Handles.color = Color.magenta;
			Handles.DrawLine(firstPoint + centerPoint / 2, firstPoint + centerPoint / 2 + Quaternion.Euler(0, flipDirection ? 90 : -90, 0) * centerPoint.normalized);
			return;
		}

		if(segment.linkPoints != null) {
			Handles.matrix = segment.transform.localToWorldMatrix;
			Undo.RecordObject(target, "Manipulate Handles");
			foreach (var point in segment.linkPoints) {
				Vector3 origPos = point.position;
				Quaternion origRot = point.rotation;
				
				EditorGUI.BeginChangeCheck();
				Handles.TransformHandle(ref origPos, ref origRot);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(target, "Change Connection Points Transform");
					point.position = origPos;
					point.rotation = origRot;
				}
			}
		}
	}
}