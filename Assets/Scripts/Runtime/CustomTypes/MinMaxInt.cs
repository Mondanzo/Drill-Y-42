using System;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class MinMaxInt {
	[SerializeField] private int minValueLimit = 0;
	[SerializeField] private int maxValueLimit = 10;
	[SerializeField] private bool expanded;
	
	public int minValue = 2;
	public int maxValue = 6;

	public MinMaxInt(int minLimit, int maxLimit, int defaultMin = 0, int defaultMax = 1) {
		minValueLimit = minLimit;
		maxValueLimit = maxLimit;
		minValue = defaultMin;
		maxValue = defaultMax;
	}
	
	public int GetInRange() {
		return Random.Range(minValue, maxValue);
	}
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(MinMaxInt))]
public class MinMaxIntProperty : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if(property.FindPropertyRelative("expanded").boolValue) {
			return base.GetPropertyHeight(property, label) * 2;
		} else {
			return base.GetPropertyHeight(property, label);
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		position.height /= 2;
		var foldoutHeaderGroupRect = new Rect(position.x, position.y, position.width, position.height);
		EditorGUI.BeginProperty(foldoutHeaderGroupRect, label, property);

		property.FindPropertyRelative("expanded").boolValue = EditorGUI.BeginFoldoutHeaderGroup(foldoutHeaderGroupRect, property.FindPropertyRelative("expanded").boolValue, label);
		EditorGUI.EndFoldoutHeaderGroup();
		SerializedProperty tempMinValueProperty = property.FindPropertyRelative("minValue");
		SerializedProperty tempMaxValueProperty = property.FindPropertyRelative("maxValue");

		int minLimit = property.FindPropertyRelative("minValueLimit").intValue;
		int maxLimit = property.FindPropertyRelative("maxValueLimit").intValue;

		float tempMinValue = tempMinValueProperty.intValue;
		float tempMaxValue = tempMaxValueProperty.intValue;
		EditorGUI.BeginChangeCheck();
		if(property.FindPropertyRelative("expanded").boolValue) {
			var minRect = new Rect(position.x, position.y + position.height, 60, position.height);
			var sliderRect = new Rect(position.x + 70, position.y + position.height, position.width - 70*2, position.height);
			var maxRect = new Rect(position.x + position.width - 60, position.y + position.height, 60, position.height);
			
			tempMinValue = EditorGUI.IntField(minRect, Mathf.FloorToInt(tempMinValue));
			EditorGUI.MinMaxSlider(sliderRect, ref tempMinValue, ref tempMaxValue, minLimit, maxLimit);
			tempMaxValue = EditorGUI.IntField(maxRect, Mathf.FloorToInt(tempMaxValue));
		}
		if(EditorGUI.EndChangeCheck()) {
			tempMinValueProperty.intValue = Mathf.FloorToInt(tempMinValue);
			tempMaxValueProperty.intValue = Mathf.FloorToInt(tempMaxValue);
		}
		
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();

	}
}
#endif