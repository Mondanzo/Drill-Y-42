using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersistentStorage : MonoBehaviour {
	public static void SetBool(string key, bool state) {
		PlayerPrefs.SetInt(key, state ? 1 : 0);
	}


	public static void SetFloat(string key, float state) {
		PlayerPrefs.SetFloat(key, state);
	}


	public static bool GetBool(string key) {
		return PlayerPrefs.GetInt(key) > 0;
	}


	public static float GetFloat(string key) {
		return PlayerPrefs.GetFloat(key);
	}
}