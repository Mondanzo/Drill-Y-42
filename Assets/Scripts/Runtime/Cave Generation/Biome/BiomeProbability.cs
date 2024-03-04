using System;
using UnityEngine;

[Serializable]
public struct BiomeProbability {
	public Biome Biome;
	public AnimationCurve Probability;
}