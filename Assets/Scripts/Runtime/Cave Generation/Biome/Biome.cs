using UnityEngine;

[Icon("Assets/Gizmos/Biome Icon.png")]
[CreateAssetMenu(fileName = "Biome", menuName = "Cave Generation/Biome")]
public class Biome : ScriptableObject {
	public Color biomeTint = Color.white;
}