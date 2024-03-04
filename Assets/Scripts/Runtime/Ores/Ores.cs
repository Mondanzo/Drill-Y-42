using System.Collections.Generic;
using UnityEngine;

public class Ores : MonoBehaviour {
	[Header("Settings")]
	[SerializeField]
	[Range(0.0F, 10.0F)]
	public float mineDuration = 2.0f;
	[SerializeField]
	[Range(0.0F, 10.0F)]
	private float oreShakeOffset = 0.05f;
	[SerializeField]
	private float oreShakeEachStep = 0.05f;
	[SerializeField]
	private List<GameObject> droppedOrePrefabs = new List<GameObject>();

	public int oreDropMin = 1;
	public int oreDropMax = 3;
	[HideInInspector]
	public float oreMultiplier = 1;
	[HideInInspector]
	public float miningSpeedMultiplier = 1;
	[SerializeField]
	private float randomForceMultiplier = 3f;
	[SerializeField]
	private float upwardsForceMultiplier = 2.5f;

	[SerializeField]
	private List<GameObject> destructionStages = new List<GameObject>();

	private float glowMax = 5f;
	private float mineProgress = 0.0f;
	private float overallMineProgress = 0.0f;
	private float stageCount = 0.0f;
	private Vector3 originalPosition; 

	void Start() {
		originalPosition = transform.position;

		stageCount = destructionStages.Count;
		if (destructionStages.Count > 0) {
			if (destructionStages[0].TryGetComponent<Renderer>(out var renderer)) {
				if (renderer.material.HasFloat("_NormalCrystalGlowIntensity")) {
					glowMax = renderer.material.GetFloat("_NormalCrystalGlowIntensity");
				}
			}
		}
	}

	private void mineOre() {
		mineProgress += Time.deltaTime;

		decreaseGlow();

		if (mineProgress % oreShakeEachStep >= oreShakeEachStep - 0.1f) {
			ShakeOre();
		}

		if (mineProgress >= (mineDuration / miningSpeedMultiplier)) {
			if (destructionStages.Count > 1) {
				mineProgress = 0f;
				dropShard(destructionStages[0]);
			} else {
				dropShard(gameObject);
			}
		}
	}

	private void decreaseGlow() {
		overallMineProgress += Time.deltaTime;
		foreach (var item in destructionStages) {
			if (item.TryGetComponent<Renderer>(out var renderer)) {
				if (renderer.material.HasFloat("_NormalCrystalGlowIntensity")) {
					renderer.material.SetFloat("_NormalCrystalGlowIntensity", Mathf.Lerp(glowMax, 0, overallMineProgress / (mineDuration * stageCount)));
				}
			}
		}
	}

	private void dropShard(GameObject obj) {
		var centerPos = obj.transform.position;
		if (obj.transform.TryGetComponent<Renderer>(out var renderer)) {
			centerPos = renderer.bounds.center;
		}

		var oreDropAmount = Random.Range(oreDropMin, oreDropMax + 1) * oreMultiplier;
		for (int i = 0; i < Mathf.Round(oreDropAmount); i++) {
			var droppedOrePrefab = droppedOrePrefabs[Random.Range(0, droppedOrePrefabs.Count)];
			GameObject oredrop = Instantiate(droppedOrePrefab, centerPos, Quaternion.identity * Random.rotation);
			oredrop.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * randomForceMultiplier + Vector3.up * upwardsForceMultiplier, ForceMode.Impulse);
		}
		
		destructionStages.Remove(obj);
		Destroy(obj);
	}

	public void Mine(GameObject other) {
		var playerData = other.GetComponent<PlayerData>();
		oreMultiplier = playerData.OreMultiplier;
		miningSpeedMultiplier = playerData.MiningSpeedMultiplier;

		mineOre();
	}

	public void InstantMine() {
		oreMultiplier = 0.2f;
		miningSpeedMultiplier = 100000f;

		mineOre();
	}

	public void ShakeOre() {
		transform.position = Vector3.Lerp(transform.position, originalPosition + Random.insideUnitSphere * oreShakeOffset, 0.5f);
	}
}
