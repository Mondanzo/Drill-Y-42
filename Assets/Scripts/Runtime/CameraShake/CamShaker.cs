using UnityEngine;


public class CamShaker : MonoBehaviour {
	private Quaternion previousRotation;

	[Range(0, 1)] public float shakeImpact = 0;
	
	[Min(0)]
	public float intensity = 1;

	[Min(0)]
	public float speed = 10;

	private void Start() {
		previousRotation = transform.rotation;
	}

	private void Update() {
		transform.rotation = previousRotation;
	}

	
	private void LateUpdate() {
		previousRotation = transform.rotation;
		Screenshake();
	}


	private void Screenshake() {
		transform.Rotate(Vector3.forward, (Mathf.PerlinNoise(Time.time * speed, 0) - 0.5f) * 2 * intensity * shakeImpact);
		transform.Rotate(Vector3.right, (Mathf.PerlinNoise(Time.time * speed, 5) - 0.5f) * 2 * intensity * shakeImpact);
		transform.Rotate(Vector3.up, (Mathf.PerlinNoise(Time.time * speed, 10) - 0.5f) * 2 * intensity * shakeImpact);
	}
	
}