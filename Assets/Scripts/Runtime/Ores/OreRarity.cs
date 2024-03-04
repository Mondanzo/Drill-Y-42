using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Ore Rarity", menuName = "Cave Generation/Ore Rarity")]
public class OreRarity : ScriptableObject {
	public Ores[] Ore;
	public AnimationCurve Rarity = AnimationCurve.Linear(0, 1, 0, 1);
	public MinMaxInt DropRate = new MinMaxInt(0, 10);
}