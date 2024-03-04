
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;


public class RNGTest : EditorWindow {

	struct GenerationResult {
		public float seed;
		public int count;
		public float value;

		public override string ToString() {
			return value + " (" + count + ")" + " - " + seed;
		}
	}

	private float seed;
	private float prevSeed;

	private List<GenerationResult> randomValues = new List<GenerationResult>();
	private SeededGenerator rng;
	public int overrideCount;
	private int numsToGenerate = 1;
	private bool useOverride = false;

	private EditorCoroutine currentCoro;
	
	[MenuItem("Window/RNG Test")]
	public static void Show() {
		RNGTest wnd = CreateWindow<RNGTest>();
		wnd.titleContent = new GUIContent("RNG Test");
	}


	public void OnEnable() {
		rng = new SeededGenerator();
		seed = rng.seed;
	}


	public void OnGUI() {
		seed = EditorGUILayout.FloatField("Seed", seed);
		numsToGenerate = EditorGUILayout.IntField("Generate so many Number(s)", numsToGenerate);
		
		GUILayout.Label("Count override");
		EditorGUILayout.BeginHorizontal();
		useOverride = EditorGUILayout.Toggle(useOverride);
		overrideCount = EditorGUILayout.IntField(overrideCount);
		EditorGUILayout.EndHorizontal();

		if (seed != prevSeed) {
			prevSeed = seed;
			rng = new SeededGenerator(seed);
		}

		if (GUILayout.Button("Reset RNG")) {
			rng = new SeededGenerator(seed);
		}

		if (GUILayout.Button("Clear List")) {
			randomValues.Clear();
		}
		
		if (GUILayout.Button("Random Number")) {
			GenerateNumbers();
		}
		
		if(GUILayout.Button("Random Number Async")) {
			if(currentCoro != null) EditorCoroutineUtility.StopCoroutine(currentCoro);
			currentCoro = EditorCoroutineUtility.StartCoroutine(GenerateNumbersAsync(), this);
		}

		int count = 0;

		while(randomValues.Count > 20) {
			randomValues.RemoveAt(0);
		}
		
		foreach (var val in randomValues) {
			GUILayout.Label(count + "> " + val.ToString());
			count++;
		}
	}
	
	
	private void GenerateNumbers() {
		for (int i = 0; i < numsToGenerate; i++) {
			var value = new GenerationResult();
			value.seed = seed;

			if (useOverride) rng.generationCounter = overrideCount - 1;

			value.value = rng.value;
			value.count = rng.generationCounter;

			if (i < numsToGenerate - 20) continue;
			randomValues.Add(value);
		}
	}
	
	
	private IEnumerator GenerateNumbersAsync() {
		for (int i = 0; i < numsToGenerate; i++) {
			var value = new GenerationResult();
			value.seed = seed;

			if (useOverride) rng.generationCounter = overrideCount - 1;

			value.value = rng.value;
			value.count = rng.generationCounter;

			if (i < numsToGenerate - 20) continue;
			randomValues.Add(value);
			yield return null;
		}
	}
}