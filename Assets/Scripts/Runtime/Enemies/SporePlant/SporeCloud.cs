using UnityEngine;
using UnityEngine.VFX;

public class SporeCloud : MonoBehaviour {

    [Header("References")]
    [SerializeField]
    private GameObject cloud;
    private SphereCollider sphereCollider;

	[Header("Settings")]
	[SerializeField]
	private float radius = 10;
    [SerializeField]
    private float damagePerSecond = 1f;
    [SerializeField]
    private float lifeTime = 10f;

    [SerializeField]
    private float initialSizeMultiplier = 0.5f;
    private float sizeMultiplier;
    private float sizeIncrement;
    [SerializeField]
    private float secondsUntilMax = 4f;

	private Vector3 cloudScale = Vector3.zero;

	private void Start() {
        Destroy(gameObject, lifeTime);

        sphereCollider = GetComponent<SphereCollider>();

		cloudScale = new Vector3(radius, radius, radius) * 2;

		sizeMultiplier = initialSizeMultiplier;
        var restSize = 1.0f - initialSizeMultiplier;
        sizeIncrement = restSize / secondsUntilMax;

        updateSize();
    }

    private void FixedUpdate() {
        sizeMultiplier = Mathf.Clamp01(sizeMultiplier + sizeIncrement * Time.deltaTime);
        updateSize();
    }

    private void updateSize() {
        sphereCollider.radius = radius * sizeMultiplier;
        cloud.transform.localScale = cloudScale * sizeMultiplier;
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerHealth>().TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

	private void OnValidate() {
		GetComponent<SphereCollider>().radius = radius;
		cloud.transform.localScale = new Vector3(radius, radius, radius) * 2;

		var sporeVfx = GetComponentInChildren<VisualEffect>();
		sporeVfx.SetFloat("Radius", radius);
		sporeVfx.SetFloat("Lifetime", lifeTime);
	}
}