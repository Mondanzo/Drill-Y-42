using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private List<AudioClip> songs = new List<AudioClip>();

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = songs[Random.Range(0, songs.Count)];
        audioSource.Play();
    }

    private void Update() {
        if (!audioSource.isPlaying) {
            audioSource.clip = songs[Random.Range(0, songs.Count)];
            audioSource.Play();
        }
    }
}
