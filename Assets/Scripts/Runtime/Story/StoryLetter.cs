using UnityEngine;


[CreateAssetMenu(fileName = "StoryLetter", menuName = "Story/Letter", order = 0)]
public class StoryLetter : ScriptableObject {
	[Multiline] public string Content;
}