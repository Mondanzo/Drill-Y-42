#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(DrillTunnelSegment))]
public class DrillTunnelSegmentEditor : Editor {
	public void OnSceneGUI() {
		var obj = target as DrillTunnelSegment;
		
		Handles.matrix = obj.transform.localToWorldMatrix;
		
		Vector3 newStartPosition = Handles.PositionHandle(obj.startPosition, Quaternion.identity);
		Vector3 newEndPosition = Handles.PositionHandle(obj.endPosition, Quaternion.identity);
		
		
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(obj, "Change Start and End Positions");
			obj.startPosition = newStartPosition;
			obj.endPosition = newEndPosition;
		}
	}
}
#endif