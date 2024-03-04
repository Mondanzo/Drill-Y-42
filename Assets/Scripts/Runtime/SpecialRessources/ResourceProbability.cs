using System;
using UnityEngine;

[Serializable]
public struct ResourceProbability {
	public RessourceType Type;
	public AnimationCurve Probability;
	public MinMaxInt Limits;
}