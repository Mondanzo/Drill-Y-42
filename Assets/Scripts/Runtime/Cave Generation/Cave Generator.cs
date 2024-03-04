using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using GUID = System.Guid;

public class CaveGenerator : CaveSegment {

	public CaveGenerationSettings generationSettings;
	private CaveGenerationSettings.SerializedGenerationSettings origSettings;

	private CamShaker camShaker;
	public GameObject playerInCave = null;
	private List<CaveSegment> generatedSegments = new List<CaveSegment>();

	private bool isCulled = false;

	public UnityEvent<GameObject> PlayerEnteredCave;
	public UnityEvent<GameObject> PlayerExitedCave;

	private bool canSentEntered = false;
	private bool canSentExited = false;

	private GameObject prevPlayer;
	private DrillTunnelGenerator generator;
	

	private void Start() {
		StartCoroutine(Generate());
		
		generator = GetComponentInParent<DrillTunnelGenerator>();
	}


	private void Update() {
		playerInCave = FindPlayerInCave();

		SentPlayerInCaveEvents();
		
		CalculateCamShake();
	}
	
	
	private void CalculateCamShake() {
		if (playerInCave) {
			if (generator) {
				var distanceToWall = Mathf.Abs(generator.collapsableOffset - (linkPoints[0].position + transform.position).z);
				camShaker = playerInCave.GetComponentInChildren<CamShaker>();

				if (camShaker) {
					camShaker.shakeImpact = Mathf.Clamp01(generationSettings.CollapsingWallShakeImpact.Evaluate(distanceToWall));
				}

				if (distanceToWall < 1) {
					var death = playerInCave.GetComponent<PlayerDeath>();
					death.Kill("Crushed in a cave.");
					death.ShowDeathScreen();
				}
			}
		} else {
			if (camShaker) {
				camShaker.shakeImpact = 0;
				camShaker = null;
			}
		}
	}
	
	
	private void SentPlayerInCaveEvents() {
		if (playerInCave) {
			canSentExited = true;

			if (canSentEntered) {
				canSentEntered = false;
				if (PlayerEnteredCave != null) PlayerEnteredCave.Invoke(playerInCave);
				prevPlayer = playerInCave;
			}
		} else {
			canSentEntered = true;

			if (canSentExited) {
				canSentExited = false;
				if (PlayerExitedCave == null) PlayerExitedCave.Invoke(prevPlayer);
				prevPlayer = null;
			}
		}
	}


	private GameObject FindPlayerInCave() {
		foreach (var segment in generatedSegments) {
			if(segment == this) continue;
			var player = segment.GetPlayerInSegment();

			if (player) {
				return player;
			}
		}

		return null;
	}

	
	public void CullCave() {
		if (isCulled) return;
		if (playerInCave != null) return; // Don't cull if player is in cave.
		
		foreach (var segment in generatedSegments) {
			if (segment == this) continue;

			CullSegment(segment, false);
		}

		isCulled = true;
	}
	
	
	private static void CullSegment(CaveSegment segment, bool show) {
		foreach (var rend in segment.GetComponentsInChildren<Renderer>()) {
			rend.enabled = show;
		}
	}

	
	public void UncullCave() {
		if (!isCulled) return;
		
		foreach (var segment in generatedSegments) {
			if (segment == this) continue;

			CullSegment(segment, true);
		}

		isCulled = false;
	}


	class A {
		public List<int> aas;
	}
	

	// Start is called before the first frame update
	public IEnumerator Generate() {
		
		#if UNITY_EDITOR
		if (placedUniqueDecorations != null) placedUniqueDecorations.Clear();
		else placedUniqueDecorations = new HashSet<Guid>();
		#else
		placedUniqueDecorations = new List<Guid>();
		#endif
		
		origSettings = generationSettings.GetSettings();
		var currentSettings = origSettings;
		
		Queue<CaveSegment> nextSegments = new Queue<CaveSegment>();
		nextSegments.Enqueue(this);

		IEnumerator editA(A a) {
			for(int i = 0; i < 5; i++) {
				a.aas.Add(i);
				yield return null;
			}
		}
		
		var a = new A();
		a.aas = new List<int>();
		
		do {
			var nextSegment = nextSegments.Dequeue();

			usedSettings = currentSettings;
			yield return PopulateSegments(nextSegment);


			StartCoroutine(editA(a));
			Debug.Log("A: " + a.aas.Count);
			
			
			var tSegments = nextSegment.LinkNext(currentSettings, transform);
			foreach (var segment in tSegments) {
				nextSegments.Enqueue(segment);
			}

			generatedSegments.Add(nextSegment);
			currentSettings.GenerationCurrency = Mathf.Max(currentSettings.GenerationCurrency - nextSegment.SegmentValue, 0);
			
			if(Application.isPlaying) if(playerInCave == null && nextSegment != this) CullSegment(nextSegment, false);
			yield return null;
		} while(nextSegments.Count > 0);
		
		if(Application.isPlaying) CullCave();
	}
	
	
	private IEnumerator PopulateSegments(CaveSegment nextSegment) {
		yield return PlaceDangers(nextSegment);
		PlaceUniqueDecorations(nextSegment);
		
		StartCoroutine(PlaceDecorations(nextSegment));
		
		StartCoroutine(FillSegmentWithOres(nextSegment));
		StartCoroutine(PlaceResources(nextSegment));
	}

	private IEnumerator FillSegmentWithOres(CaveSegment current) {

		var ores = current.GetComponentsInChildren<RandomOre>(true).ToList();
		ShuffleList(ref ores);
		PlaceOres(ores, current);
		yield return null;
	}


	private void PlaceOres(List<RandomOre> ores, CaveSegment segment) {

		foreach (var ore in ores) {
			#if UNITY_EDITOR
			if (!EditorApplication.isPlaying) {
				ore.gameObject.SetActive(false);
			}
			#endif
			var orePlaced = false;
			
			var threshhold = usedSettings.LootProbability.Evaluate(segment.GetCurrentDepthPercentage());
			
			if (threshhold > Random.value) {
				var rarities = usedSettings.OreRarities;
				ShuffleList(ref rarities);
				foreach (var rarity in rarities) {
					var list = rarity.Ore.ToList();
					ShuffleList(ref list);
					foreach (var o in rarity.Ore) {
						if (!ore.PossibleOres.Contains(o)) continue;
						var odds = rarity.Rarity.Evaluate(segment.GetCurrentDepthPercentage());
					
						if (odds > Random.value) {
							o.oreDropMin = rarity.DropRate.minValue;
							o.oreDropMax = rarity.DropRate.maxValue;

							ore.PlaceOre(o);
							orePlaced = true;
							break;
						}
					}
				}
			}
			
			if(orePlaced) {
				ore.gameObject.SetActive(true);
			} else {
				DestroyImmediate(ore.gameObject);
			}
		}
	}


	private IEnumerator PlaceResources(CaveSegment current) {
		
		#if UNITY_EDITOR
		if (!EditorApplication.isPlaying) {
			foreach (var rr in current.GetComponentsInChildren<RandomRessource>()) {
				rr.gameObject.SetActive(false);
			}
		}
		#endif
		
		foreach (var resource in usedSettings.ResourceProbabilities) {
			List<RandomRessource> currentResources = null;
			if (resource.Probability.Evaluate(current.GetCurrentDepthPercentage()) > Random.value) {
				currentResources = current.GetComponentsInChildren<RandomRessource>(true).Where(r => r.RessourceType == resource.Type).ToList();
				ShuffleList(ref currentResources);
				int place = resource.Limits.GetInRange();
				foreach (var rr in currentResources) {
					if (place <= 0) break;
					rr.gameObject.SetActive(true);
					place--;
				}
			}

			if (currentResources != null) {
				foreach (var R in currentResources) {
					if(R) if(!R.gameObject.activeInHierarchy) DestroyImmediate(R.gameObject);
				}
			}

			yield return null;
		}
	}


	private IEnumerator PlaceDecorations(CaveSegment current) {

		var decorations = current.GetComponentsInChildren<RandomDecorationElement>().ToList();
		
		foreach (var deco in decorations) {
			deco.gameObject.SetActive(false);
		}

		ShuffleList(ref decorations);
		foreach (var decoration in decorations) {
			if (decoration.SpawnRarity >= Random.value) {
				decoration.PlaceDecoration();
			}
			
			// foreach (var biomeProbability in usedSettings.BiomeProbability) {
			// 	if (decoration.Biome.Contains(biomeProbability.Biome)) {
			// 		var odds = biomeProbability.Probability.Evaluate(current.GetCurrentDepthPercentage());
			// 		if (odds >= Random.value) {
			// 			if (decoration.SpawnRarity >= Random.value) {
			// 				decoration.PlaceDecoration(current);
			// 				break;
			// 			}
			// 		}
			// 	}
			// }
			yield return null;
		}
	}


	private HashSet<GUID> placedUniqueDecorations;
	
	private void PlaceUniqueDecorations(CaveSegment current) {
		var possibleDecos = current.GetComponentsInChildren<RandomUniqueDecorationElement>(true);

		if (possibleDecos.Length <= 0) return;

		
		List<RandomUniqueDecorationElement> possibleElements = new List<RandomUniqueDecorationElement>();
		
		foreach (var deco in possibleDecos) {
			if(!deco.gameObject.activeInHierarchy) continue;
			deco.gameObject.SetActive(false);
			if(!placedUniqueDecorations.Contains(deco.uniqueId)) possibleElements.Add(deco);
		}
		
		if (possibleElements.Count > 0) {
			var deco = possibleElements.ElementAt(Random.Range(0, possibleElements.Count()));
			deco.PlaceUniqueDecoration();
			placedUniqueDecorations.Add(deco.uniqueId);
		}
	}


	private IEnumerator PlaceDangers(CaveSegment current) {

		var dangers = current.GetComponentsInChildren<RandomDanger>(true).ToList();
		
		foreach (var danger in dangers) {
			danger.gameObject.SetActive(false);
		}
		
		bool isPeaceful = true;

		ShuffleList(ref dangers);
		foreach (var danger in dangers) {
			var odds = usedSettings.Danger.Evaluate(current.GetCurrentDepthPercentage());

			if (odds >= Random.value) {
				danger.Place();

				if (isPeaceful) {
					isPeaceful = false;

					foreach (var peacefulness in current.GetComponentsInChildren<IfPeaceful>()) {
						peacefulness.KillThePeace();
					}
				}
				yield break;
			}
			DestroyImmediate(danger.gameObject);
		}
		yield return null;
	}


	private void OnDestroy() {
		if (playerInCave) {
			if(Application.isPlaying) {
				var death = playerInCave.GetComponent<PlayerDeath>();
				death.Kill("Crushed to death.");
				death.ShowDeathScreen();
			}
		}
	}
}