using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


[CustomEditor(typeof(SubtitledAudioClip))]
public class SubtitlesEditor : Editor {

	private SerializedProperty audioClip;
	private SerializedProperty subtitles;
	
	private float[] data;
	private uint clipHash;
	private List<Color> colors;
	
	private AudioClip currentLoaded;
	private bool loading, previewLoaded;
	
	private Sound currentSound;
	private Channel playbackChannel;

	private float playbackTime;

	private FMOD.Studio.System studioSystem;
	private FMOD.System coreSystem;
	
	private int currentSelected = 0;

	public void OnEnable() {
		audioClip = serializedObject.FindProperty("clip");
		subtitles = serializedObject.FindProperty("subtitles");
		colors = new List<Color>();

		for (int i = 0; i < subtitles.arraySize; i++) {
			addRandomColor();
		}

		studioSystem = EditorUtils.System;
		studioSystem.getCoreSystem(out coreSystem);
		
		unloadClip();
	}

	private void addRandomColor() {
		colors.Add(Random.ColorHSV(0, 1, 0.8f, 1, 0.4f, 0.6f));
	}


	public void OnDisable() {
		unloadClip();
	}


	private bool previewPlaying() {
		bool playing = false;
		if (playbackChannel.hasHandle()) {
			playbackChannel.isPlaying(out playing);
			bool paused;
			playbackChannel.getPaused(out paused);
			playing = playing && !paused;
		}

		return playing;
	}


	private void loadClip() {
		if (currentLoaded) return;

		if(audioClip.boxedValue is AudioClip clip) {
			var audioFilePath = AssetDatabase.GetAssetPath(clip);

			if (clip.loadState == AudioDataLoadState.Unloaded) {
				clip.LoadAudioData();
				loading = true;
			}

			if (clip.loadState == AudioDataLoadState.Loaded) {
				data = new float[clip.samples * clip.channels];
				clip.GetData(data, 0);
				clipHash = audioClip.contentHash;
				currentLoaded = clip;
				clip.UnloadAudioData();

				var result = coreSystem.createSound(audioFilePath, MODE.CREATECOMPRESSEDSAMPLE, out currentSound);
				if (result == RESULT.OK) {
					previewLoaded = true;
				} else {
					previewLoaded = false;
					Debug.LogError("Failed to load preview. Reason: " + result);
				}
				
				loading = false;
			}
		}
	}


	private void unloadClip() {
		if (playbackChannel.hasHandle()) {
			playbackChannel.stop();
			playbackChannel.clearHandle();
			playbackTime = 0;
		}

		if (currentSound.hasHandle()) {
			currentSound.release();
			currentSound.clearHandle();
		}

		previewLoaded = false;
		clipHash = 0;
		currentLoaded = null;
	}

	public override bool RequiresConstantRepaint() {
		return base.RequiresConstantRepaint() || loading || previewPlaying();
	}


	public override void OnInspectorGUI() {
		if(audioClip.boxedValue is AudioClip) loadClip();
		if (previewLoaded) {
			if(data != null && data.Length > 0) {
				DrawSubtitlesWindow();
			}
		}
		
		if (DrawDefaultInspector()) {
			AdjustColorArray();

			if (clipHash != audioClip.contentHash) {
				unloadClip();
				loadClip();
			}
		}
	}
	
	
	private void AdjustColorArray() {
		if (subtitles.arraySize > colors.Count) {
			for (int i = colors.Count; i < subtitles.arraySize; i++) {
				addRandomColor();
			}
		} else if (subtitles.arraySize < colors.Count) {
			var c = colors.Count;

			for (int i = subtitles.arraySize; i < c; i++) {
				colors.RemoveAt(0);
			}
		}
	}
	
	
	private void DrawSubtitlesWindow() {
		var waveformRect = new Rect(0, 0, EditorGUIUtility.currentViewWidth, 100);
		
		DrawAudio(waveformRect);
		DrawSubtitlesOverlay(waveformRect);
		
		GUILayout.Space(waveformRect.height);

		if (previewLoaded) DrawPlaybackControls();

		GUILayout.Label(((SubtitledAudioClip) target).GetCurrentSubtitle(playbackTime));
		playbackTime = EditorGUILayout.Slider(playbackTime, 0, currentLoaded.length);
	}

	
	private void DrawPlaybackControls() {
		var playing = previewPlaying();

		if (playing) {
			uint pos;

			if (playbackChannel.getPosition(out pos, TIMEUNIT.MS) == RESULT.OK) {
				playbackTime = (float) pos / 1000;
			}
		}

		GUILayout.BeginHorizontal();
		if (GUILayout.Button(EditorGUIUtility.IconContent(playing ? "PauseButton On" : "PlayButton On"))) {
			if(!playbackChannel.hasHandle()) {
				ChannelGroup channelGroup;

				if (coreSystem.getMasterChannelGroup(out channelGroup) == RESULT.OK) {
					var result = coreSystem.playSound(currentSound, channelGroup, false, out playbackChannel);
					if(result != RESULT.OK) Debug.LogError("Couldn't play sound. Reason: " + result);
					
				} else {
					Debug.LogError("Couldn't get master channel group.");
				}
			}

			var result2 = playbackChannel.setPosition((uint) Mathf.RoundToInt(playbackTime * 1000), TIMEUNIT.MS);
			if(result2 == RESULT.OK) {
				result2 = playbackChannel.setPaused(playing);

				if (result2 != RESULT.OK) {
					Debug.LogError("Couldn't toggle pause state of preview. Reason: " + result2);
				}
			} else {
				Debug.LogError("Couldn't set preview position. Reason: " + result2);
			}
		}

		if (GUILayout.Button(EditorGUIUtility.IconContent("d_beginButton"))) {
			if (playbackChannel.hasHandle()) {
				playbackChannel.stop();
				playbackChannel.clearHandle();
				playbackTime = 0;
			}
		}
		GUILayout.EndHorizontal();
	}

	
	private void DrawAudio(Rect waveformRect) {
		AudioCurveRendering.BeginCurveFrame(waveformRect);

		AudioCurveRendering.DrawCurve(
			waveformRect,
			f => { return data[Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(0, data.Length, f)), 0, data.Length - 1)]; },
			Color.yellow
		);

		AudioCurveRendering.EndCurveFrame();
	}
	private void DrawSubtitlesOverlay(Rect waveformRect) {
		var iterator = subtitles.GetEnumerator();

		int i = 0;

		while(iterator.MoveNext()) {
			var subtitle = iterator.Current as SerializedProperty;
			var startPercentage = subtitle.FindPropertyRelative("startTime").floatValue / currentLoaded.length;
			var endPercentage = subtitle.FindPropertyRelative("endTime").floatValue / currentLoaded.length;
			var subtitleRect = new Rect(waveformRect.width * startPercentage, 5, waveformRect.width * endPercentage - waveformRect.width * startPercentage, 25);
			EditorGUI.DrawRect(subtitleRect, colors[i++]);
			EditorGUI.DrawRect(new Rect(waveformRect.width * playbackTime / currentLoaded.length, 0, 1, waveformRect.height), Color.black);
			GUI.Label(subtitleRect, subtitle.FindPropertyRelative("subtitleText").stringValue);
		}
	}
}