
using System;
using UnityEngine;
using Random = UnityEngine.Random;



public class RandomChitChat : MonoBehaviour {

	public SubtitlePositionalPlayer player;
	
	public VoiceMemoOrders chatBeforeChitChat;
	public VoiceMemoOrders chitChat;

	private int lastPlayedChitChatIndex = -1;
	
	private void Start() {
		player = GetComponent<SubtitlePositionalPlayer>();
	}


	public void PlayChat() {
		Debug.Log("playing chit chat");
		if (chatBeforeChitChat.HasMemosLeft()) {
			player.PlayClip(chatBeforeChitChat.GetNextMemo(), true);
		} else {
			PlayChitChat();
		}
	}


	public void PlayChitChat() {
		var idxToPlay = -1;

		do {
			idxToPlay = Random.Range(0, chitChat.voiceMemos.Count);

			if (chitChat.voiceMemos.Count < 2) break;
		} while(idxToPlay == lastPlayedChitChatIndex);

		lastPlayedChitChatIndex = idxToPlay;
		
		player.PlayClip(chitChat.voiceMemos[lastPlayedChitChatIndex]);
	}
}