using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryLetterSet", menuName = "Story/Letter Set")]
public class StoryLetterOrder : ScriptableObject {
	public List<StoryLetter> storyLetters;

	public static Dictionary<StoryLetterOrder, Queue<StoryLetter>> StoryLettersDict;

	public StoryLetter GetNextLetter() {
		if(StoryLettersDict[this].TryDequeue(out var letter)) return letter;
		
		return null;
	}


	public void OnEnable() {
		Setup();
	}


	public void OnDisable() {
		if (StoryLettersDict != null && StoryLettersDict.ContainsKey(this)) StoryLettersDict.Remove(this);
	}


	private void Setup() {
		if (StoryLettersDict == null) {
			StoryLettersDict = new Dictionary<StoryLetterOrder, Queue<StoryLetter>>();
		}

		if (!StoryLettersDict.ContainsKey(this)) {
			var tQueue = new Queue<StoryLetter>();

			foreach (var letter in storyLetters) {
				tQueue.Enqueue(letter);
			}

			StoryLettersDict.Add(this, tQueue);
		}
	}


	public bool HasLettersLeft() {
		Queue<StoryLetter> tQueue;
		if (StoryLettersDict.TryGetValue(this, out tQueue)) return tQueue.Count > 0;
		return false;
	}

	public bool Exists() {
		return StoryLettersDict.ContainsKey(this);
	}
}