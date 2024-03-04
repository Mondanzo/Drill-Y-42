using System.Linq;
using UnityEngine;


public class InterpolatingAnimationCurve : AnimationCurve {
	private AnimationCurve second;
	private AnimationCurve first;
	private float weight;

	public InterpolatingAnimationCurve(AnimationCurve self, AnimationCurve other, float weight) {
		if (self.Equals(other)) keys = self.keys;
		for (float i = 0; i < Mathf.Max(self.keys.Last().time, other.keys.Last().time); i += 0.01f) {
			AddKey(i, Mathf.Lerp(self.Evaluate(i), other.Evaluate(i), weight));
		}
	}
}