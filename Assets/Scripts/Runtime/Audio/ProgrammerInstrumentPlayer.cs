using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;


public class ProgrammerInstrumentPlayer : MonoBehaviour {
	public FMODUnity.EventReference eventToPlay;
	public AudioClip clip;
	public STOP_MODE stopMode;

	private EventInstance currentInstance;
	private EVENT_CALLBACK programmerInstrumentCallback;

	public float overrideMinDistance = -1;
	public float overrideMaxDistance = -1;

	private void Start() {
		programmerInstrumentCallback = ProgrammerInstrumentCallback;
	}


	private void OnDestroy() {
		Cancel();
	}


	public float GetCurrentPosition() {
		if (!currentInstance.isValid()) return 0;
		ERRCHECK(currentInstance.getTimelinePosition(out var pos), "Failed to get timeline position.");
		return (float) pos / 1000;
	}


	public bool IsPlaying() {
		if (!currentInstance.isValid()) return false;
		currentInstance.getPlaybackState(out var state);

		switch (state) {
			case PLAYBACK_STATE.STARTING:
			case PLAYBACK_STATE.PLAYING:
			case PLAYBACK_STATE.STOPPING:
			case PLAYBACK_STATE.SUSTAINING:
				return true;

			case PLAYBACK_STATE.STOPPED:
				return false;
		}

		return false;
	}


	public bool IsPaused() {
		if (!currentInstance.isValid()) return false;
		currentInstance.getPaused(out var paused);
		return paused;
	}


	public void SetPaused(bool paused) {
		if (currentInstance.isValid()) {
			ERRCHECK(currentInstance.setPaused(paused), "Failed to change pause state");
		}
	}

	public void Cancel() {
		if (currentInstance.isValid()) currentInstance.stop(stopMode);
	}


	public IEnumerator PlayClip(AudioClip clip) {
		if (clip.loadState == AudioDataLoadState.Unloaded) {
			clip.LoadAudioData();
		}

		yield return new WaitUntil(() => clip.loadState == AudioDataLoadState.Loaded || clip.loadState == AudioDataLoadState.Failed);

		if (clip.loadState == AudioDataLoadState.Loaded) {
			float[] samples = new float[clip.samples * clip.channels];
			clip.GetData(samples, 0);

			FMOD.System core = FMODUnity.RuntimeManager.CoreSystem;

			uint lenbytes = (uint) (clip.samples * clip.channels * sizeof(float));

			CREATESOUNDEXINFO exinfo = new CREATESOUNDEXINFO();
			exinfo.cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>();
			exinfo.length = lenbytes;
			exinfo.format = SOUND_FORMAT.PCMFLOAT;
			exinfo.defaultfrequency = clip.frequency;
			exinfo.numchannels = clip.channels;

			var result = ERRCHECK(core.createSound("", MODE.OPENUSER, ref exinfo, out var sound), "Failed to create sound");

			if (result != RESULT.OK) yield break;
			result = ERRCHECK(sound.@lock(0, lenbytes, out var ptr1, out var ptr2, out var len1, out var len2), "Failed to lock sound.");
			if (result != RESULT.OK) yield break;
			Marshal.Copy(samples, 0, ptr1, (int) (len1 / sizeof(float)));

			if (len2 > 0)
				Marshal.Copy(samples, (int) (len1 / sizeof(float)), ptr2, (int) (len2 / sizeof(float)));

			result = ERRCHECK(sound.unlock(ptr1, ptr2, len1, len2), "Failed to unlock sound.");
			if (result != RESULT.OK) yield break;

			var eventDesc = RuntimeManager.GetEventDescription(eventToPlay);
			eventDesc.is3D(out var is3d);

			var eventInstance = RuntimeManager.CreateInstance(eventToPlay);
			if(is3d) RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
			GCHandle soundToPlay = GCHandle.Alloc(sound.handle);

			eventInstance.setUserData(GCHandle.ToIntPtr(soundToPlay));
			eventInstance.setCallback(programmerInstrumentCallback);

			ERRCHECK(eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, overrideMinDistance), "Failed to override min distance.");
			ERRCHECK(eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, overrideMaxDistance), "Failed to override max distance.");
			
			eventInstance.start();
			eventInstance.release();
			currentInstance = eventInstance;
		} else {
			Debug.LogError("Failed to play sound " + clip.name);
		}
	}


	public void EasyPlayClip(AudioClip clip) {
		StartCoroutine(PlayClip(clip));
	}


	public void PlayOwnClip() {
		EasyPlayClip(clip);
	}


	[AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
	static RESULT ProgrammerInstrumentCallback(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters) {
		EventInstance instance = new EventInstance(_event);
		if (!instance.isValid()) return RESULT.ERR_EVENT_NOTFOUND;

		instance.getUserData(out var clipDataPtr);
		GCHandle clipHandle = GCHandle.FromIntPtr(clipDataPtr);
		IntPtr soundHandle = (IntPtr) clipHandle.Target;

		switch (type) {
			case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND: {
				var param = Marshal.PtrToStructure<PROGRAMMER_SOUND_PROPERTIES>(parameters);

				param.sound = soundHandle;
				param.subsoundIndex = -1;
				Marshal.StructureToPtr(param, parameters, false);
			}

				break;

			case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND: {
				var sound = new Sound(soundHandle);
				var result = ERRCHECK(sound.release(), "Failed to release sound. That's not good.");

				if (result != RESULT.OK) return result;
				break;
			}

			case EVENT_CALLBACK_TYPE.DESTROYED:
				clipHandle.Free();
				break;
		}

		return RESULT.OK;
	}

	private static RESULT ERRCHECK(RESULT result, string failMsg) {
		#if UNITY_EDITOR
		if (result != FMOD.RESULT.OK) {
			Debug.Log(failMsg + " with result: " + result);
			return result;
		}
		#endif
		return result;
	}
}