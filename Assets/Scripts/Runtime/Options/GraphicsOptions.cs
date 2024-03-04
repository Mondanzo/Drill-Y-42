using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;


[Serializable]
public class GraphicsOptions {

	[SerializeField] private int fullscreenMode;

	[SerializeField] private int resIdx;

	[SerializeField] private int targetFps = 60;

	[FormerlySerializedAs("qualityPrefab")] [SerializeField] private int qualityPreset = 0;


	public static GraphicsOptions GetCurrent() {
		var currentOptions  = new GraphicsOptions();

		var res = Screen.currentResolution;
		currentOptions.resIdx = FindIdxForResolution(res);

		currentOptions.fullscreenMode = GetIndexByFullscreenMode(Screen.fullScreenMode);
		
		currentOptions.qualityPreset = QualitySettings.GetQualityLevel();

		return currentOptions;
	}


	public static int FindIdxForResolution(Resolution res) {
		for (int i = 0; i < Screen.resolutions.Length; i++) {
			if (Screen.resolutions[i].ToString() == res.ToString())
				return i;
		}

		return -1;
	}


	private static string getSettingsPath() {
		return Path.Join(Application.persistentDataPath, "graphicSettings.json");
	}
	
	
	public static void SaveOptions() {
		Debug.Log("Saving graphics options");
		var saveStruct = GetCurrent();
		
		var jsonData = JsonUtility.ToJson(saveStruct, true);
		Debug.Log(jsonData);

		var stream = File.CreateText(getSettingsPath());
		
		stream.Write(jsonData);
		
		stream.Flush();
		stream.Close();
	}


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void LoadOptions() {
		if (File.Exists(getSettingsPath())) {
			Debug.Log("Trying to load graphics options");
			var loadedOptions = JsonUtility.FromJson<GraphicsOptions>(File.ReadAllText(getSettingsPath()));
			if(loadedOptions != null) {
				
				Application.targetFrameRate = loadedOptions.targetFps;

				// Set Screen Resolution
				{
					var selectedIdx = -1;	
		
					if (Screen.resolutions.Length > loadedOptions.resIdx && loadedOptions.resIdx > 0) {	
						selectedIdx = loadedOptions.resIdx;	
					} else {	
						selectedIdx = Screen.resolutions.Length - 1;
					}
		
					var resolution = Screen.resolutions[selectedIdx];

					Screen.SetResolution(resolution.width, resolution.height, GetFullscreenModeByIndex(loadedOptions.fullscreenMode), resolution.refreshRateRatio);
				}
				
				// Set Quality Preset
				{
					int selectedIdx;
		
					if (Screen.resolutions.Length > loadedOptions.resIdx && loadedOptions.resIdx > 0) {	
						selectedIdx = loadedOptions.qualityPreset;
					} else {	
						selectedIdx = GraphicsSettings.allConfiguredRenderPipelines.Length - 1;
					}

					QualitySettings.SetQualityLevel(selectedIdx);
				}
			} else {
				SaveOptions();
			}
		} else {
			SaveOptions();
		}
	}


	public static void SetResolutionByIndex(int idx) {
		if (Screen.resolutions.Length > idx && idx > 0) {
			var targetRes = Screen.resolutions[idx];
			Screen.SetResolution(targetRes.width, targetRes.height, Screen.fullScreenMode, targetRes.refreshRateRatio);
		}
	}
	
	public static FullScreenMode GetFullscreenModeByIndex(int value) {
		var mode = Screen.fullScreenMode;

		switch (value) {
			case 0:
				mode = FullScreenMode.Windowed;
				break;
			case 1:
				mode = FullScreenMode.MaximizedWindow;
				break;
			case 2:
				mode = FullScreenMode.FullScreenWindow;
				break;
			case 3:
				mode = FullScreenMode.ExclusiveFullScreen;
				break;
		}

		return mode;
	}

	public static int GetIndexByFullscreenMode(FullScreenMode mode) {
		switch (mode) {
			case FullScreenMode.Windowed:
				return 0;
			case FullScreenMode.MaximizedWindow:
				return 1;
			case FullScreenMode.FullScreenWindow:
				return 2;
			case FullScreenMode.ExclusiveFullScreen:
				return 3;
		}

		return 0;
	}
}