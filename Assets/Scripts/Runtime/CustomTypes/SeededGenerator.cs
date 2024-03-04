using System;
using UnityEngine;


public class SeededGenerator {
	public int generationCounter;
	public float seed;


	public SeededGenerator() {
		seed = Mathf.PerlinNoise1D(Time.time) * 10000000f;
		generationCounter = 0;
	}


	public SeededGenerator(float seed) {
		this.seed = seed;
		generationCounter = 0;
	}
	
	
	

	
	public float value {
		get {
			generationCounter++;
			var val = Mathf.Clamp01(Mathf.PerlinNoise((float) generationCounter - seed * 0.01f, (float) generationCounter + seed * 0.01f));
			Debug.Log("value: " + val + " count: " + generationCounter + " seed: " + seed);
			return val;
		}
	}

	public int Range(int min, int max) {
		return Math.Min(Mathf.FloorToInt(min + value * max), max - 1);
	}

	public float Range(float min, float max) {
		return min + value * max;
	}
}