using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRessource : MonoBehaviour {
	public Vector3 minRotation = Vector3.zero;
	public Vector3 maxRotation = Vector3.zero;

	public RessourceType RessourceType;

	void Awake() {
		gameObject.SetActive(false);
	}

	// Start is called before the first frame update
	void Start() {
		PlaceRessource();
	}
	
	public void PlaceRessource() {
		transform.rotation *= GetRandomRotation();
	}

	private Quaternion GetRandomRotation() {
		return Quaternion.Euler(Random.Range(minRotation.x, maxRotation.x), Random.Range(minRotation.y, maxRotation.y), Random.Range(minRotation.z, maxRotation.z));
	}
}