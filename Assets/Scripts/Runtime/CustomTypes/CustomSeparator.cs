#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class CustomSeparator : PropertyAttribute {}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomSeparator))]
public class CustomSeparatorDrawer : DecoratorDrawer {
	public override void OnGUI(Rect position) {
		position.y += 5;
		position.height = 1;
		EditorGUI.DrawRect(position, new Color(0.1f, 0.1f, 0.1f, 1.0f));
		position.y++;
		EditorGUI.DrawRect(position, Color.grey);
	}

	public override float GetHeight() {
		return 5;
	}
}
#endif