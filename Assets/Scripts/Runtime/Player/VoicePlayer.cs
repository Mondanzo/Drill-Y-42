using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePlayer : MonoBehaviour {

    [SerializeField]
    private AudioSource audioSource;

    public void PlayVoiceLine(AudioClip audioClip) {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
