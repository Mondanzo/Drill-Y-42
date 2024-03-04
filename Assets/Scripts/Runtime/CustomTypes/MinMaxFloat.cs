using System;
using Random = UnityEngine.Random;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class MinMaxFloat {
	[SerializeField] private float minValueLimit;
	[SerializeField] private float maxValueLimit;
	
	public float minValue;
	public float maxValue;

	public MinMaxFloat(float minLimit, float maxLimit, float defaultMin = 0, float defaultMax = 1) {
		minValueLimit = minLimit;
		maxValueLimit = maxLimit;
		minValue = defaultMin;
		maxValue = defaultMax;
	}
	
	public float GetInRange() {
		return Random.Range(minValue, maxValue);
	}
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(MinMaxFloat))]
public class MinMaxFloatProperty : PropertyDrawer {

	private bool expanded = false;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if(expanded) {
			return base.GetPropertyHeight(property, label) * 2;
		}
		else {
			return base.GetPropertyHeight(property, label);
		}
	}
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		position.height /= 2;
		var foldoutHeaderGroupRect = new Rect(position.x, position.y, position.width, position.height);
		EditorGUI.BeginProperty(foldoutHeaderGroupRect, label, property);
		
		expanded = EditorGUI.BeginFoldoutHeaderGroup(position, expanded, label);
		EditorGUI.EndFoldoutHeaderGroup();
		SerializedProperty tempMinValueProperty = property.FindPropertyRelative("minValue");
		SerializedProperty tempMaxValueProperty = property.FindPropertyRelative("maxValue");

		float minLimit = property.FindPropertyRelative("minValueLimit").floatValue;
		float maxLimit = property.FindPropertyRelative("maxValueLimit").floatValue;

		float tempMinValue = tempMinValueProperty.floatValue;
		float tempMaxValue = tempMaxValueProperty.floatValue;
		EditorGUI.BeginChangeCheck();
		if(expanded) {
			var minRect = new Rect(position.x, position.y + position.height, 60, position.height);
			var sliderRect = new Rect(position.x + 70, position.y + position.height, position.width - 70*2, position.height);
			var maxRect = new Rect(position.x + position.width - 60, position.y + position.height, 60, position.height);
			
			tempMinValue = EditorGUI.FloatField(minRect, tempMinValue);
			EditorGUI.MinMaxSlider(sliderRect, ref tempMinValue, ref tempMaxValue, minLimit, maxLimit);
			tempMaxValue = EditorGUI.FloatField(maxRect, tempMaxValue);
		}
		if(EditorGUI.EndChangeCheck()) {
			tempMinValueProperty.floatValue = tempMinValue;
			tempMaxValueProperty.floatValue = tempMaxValue;
		}
		
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();

	}
}
#endif