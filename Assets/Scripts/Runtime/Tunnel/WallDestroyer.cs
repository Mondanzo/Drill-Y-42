using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class WallDestroyer : MonoBehaviour {
	private VisualEffect visualEffect;

	private void Start() {
		visualEffect = GetComponentInChildren<VisualEffect>();
	}

	public void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent<DestructibleWall>(out var wall)) {
			wall.Destroy();

			if (!visualEffect.HasAnySystemAwake()) visualEffect.Play();
			StopAllCoroutines();
			StartCoroutine(stopAfterDelay());
		}
	}

	private IEnumerator stopAfterDelay() {
		yield return new WaitForSeconds(1);
		visualEffect.Stop();
	}
}