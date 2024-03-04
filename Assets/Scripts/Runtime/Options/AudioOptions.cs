using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class AudioOptions {

	public static Dictionary<string, float> audioVolumes = new Dictionary<string, float>();
	
	private static string getSettingsPath() {
		return Path.Join(Application.persistentDataPath, "audioSettings.ini");
	}
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void LoadOptions() {
		var settingFile = getSettingsPath();

		if (File.Exists(settingFile)) {
			var lines = File.ReadAllLines(settingFile);

			try {
				foreach (var line in lines) {
					var parts = line.Split('=');

					if (parts.Length >= 2) {
						if (float.TryParse(parts[1], out var newVolume)) {
							Debug.Log("Loaded volume for bus " + parts[0] + "-> " + newVolume);
							audioVolumes[parts[0]] = Mathf.Clamp01(newVolume);
						} else {
							Debug.Log("Failed to parse volume for bus " + parts[0]);
						}
					}
				}
			} catch (Exception error) {
				Debug.LogError("Failed to load audio options");
			}
		}
	}


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void ApplyLoadedVolumes() {
		List<string> corruptedBuses = new List<string>();
		
		foreach (var volume in audioVolumes) {
			var bus = FMODUnity.RuntimeManager.GetBus(volume.Key);

			if (bus.isValid()) {
				bus.setVolume(volume.Value);
			} else {
				corruptedBuses.Add(volume.Key);
			}
		}

		foreach (var wrongBus in corruptedBuses) {
			audioVolumes.Remove(wrongBus);
		}
		
		SaveOptions();
	}


	public static void SaveOptions() {
		var saveFile = getSettingsPath();
		var stream = File.CreateText(saveFile);
		
		foreach (var volumeSetting in audioVolumes) {
			stream.WriteLine(volumeSetting.Key + "=" + volumeSetting.Value);
		}
		
		stream.Flush();
		stream.Close();
	}


	public static void SetVolume(string bus, float newVal) {
		audioVolumes[bus] = newVal;
		SaveOptions();
	}
}