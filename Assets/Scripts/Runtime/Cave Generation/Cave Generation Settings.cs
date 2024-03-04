using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "CaveSettings", menuName = "Cave Generation/Cave Settings")]
public class CaveGenerationSettings : ScriptableObject {

	[SerializeField] private float minGenerationCurrency = 1;
	[SerializeField] private float maxGenerationCurrency = 10;

	[SerializeField] private float minRarity = 1;
	[SerializeField] private float maxRarity = 1;
	
	[SerializeField] private AnimationCurve Danger;
	[SerializeField] public AnimationCurve CurvatureSettings;
	[SerializeField] public AnimationCurve CollapsingWallShakeImpact;


	[SerializeField] private float minValueModifier = 1;
	[SerializeField] private float maxValueModifier = 1;

	[SerializeField] private AnimationCurve LootProbability;
	[SerializeField] private List<OreRarity> OreRarities;
	
	[SerializeField] private List<CaveSegment> PossibleSegments;
	
	[SerializeField] private List<BiomeProbability> BiomeProbability;
	[SerializeField] private List<ResourceProbability> ResourceProbabilities;
	
	public struct SerializedGenerationSettings {
		public float GenerationCurrency;
		public float Rarity;
		public AnimationCurve Danger;
		public float ValueModifier;
		public AnimationCurve LootProbability;
		public AnimationCurve CurvatureSettings;
		public BlueprintPool PossibleBlueprints;
		public List<CaveSegment> PossibleSegments;
		public List<OreRarity> OreRarities;
		public List<BiomeProbability> BiomeProbability;
		public List<ResourceProbability> ResourceProbabilities;
	}
	
	public SerializedGenerationSettings GetSettings() {
		return new SerializedGenerationSettings() {
			GenerationCurrency = GenerationCurrency,
			Rarity = Rarity,
			PossibleSegments = PossibleSegments,
			Danger = Danger,
			LootProbability = LootProbability,
			ValueModifier = ValueModifier,
			OreRarities = OreRarities,
			BiomeProbability = BiomeProbability,
			ResourceProbabilities = ResourceProbabilities
		};
	}
	
	
	public float GenerationCurrency {
		get {
			return Random.Range(minGenerationCurrency, maxGenerationCurrency);
		}
	}


	public float Rarity {
		get {
			return Random.Range(minRarity, maxRarity);
		}
	}


	public float ValueModifier {
		get {
			return Random.Range(minValueModifier, maxValueModifier);
		}
	}


	public CaveGenerationSettings InterpolateSettings(CaveGenerationSettings other, float weight) {
		return InterpolateSettings(this, other, weight);
	}
	

	public static CaveGenerationSettings InterpolateSettings(CaveGenerationSettings first, CaveGenerationSettings second, float weight) {
		var retVal = ScriptableObject.CreateInstance<CaveGenerationSettings>();
		retVal.CurvatureSettings = new InterpolatingAnimationCurve(first.CurvatureSettings, second.CurvatureSettings, weight);
		retVal.minGenerationCurrency = Mathf.Lerp(first.minGenerationCurrency, second.minGenerationCurrency, weight);
		retVal.maxGenerationCurrency = Mathf.Lerp(first.maxGenerationCurrency, second.maxGenerationCurrency, weight);
		retVal.PossibleSegments = weight > 0.5f ? second.PossibleSegments : first.PossibleSegments;
		retVal.Danger = new InterpolatingAnimationCurve(first.Danger, second.Danger, weight);
		retVal.LootProbability = new InterpolatingAnimationCurve(first.LootProbability, second.LootProbability, weight);
		retVal.OreRarities = weight > 0.5f ? second.OreRarities : first.OreRarities;
		retVal.ResourceProbabilities = weight > 0.5f ? second.ResourceProbabilities : first.ResourceProbabilities;
		retVal.CollapsingWallShakeImpact = new InterpolatingAnimationCurve(first.CollapsingWallShakeImpact, second.CollapsingWallShakeImpact, weight);

		retVal.BiomeProbability = new List<BiomeProbability>();
		
		foreach (var probability in first.BiomeProbability) {
			var other = second.BiomeProbability.Where(b => b.Biome = probability.Biome);
			if(other.Any()) {
				var otherBiome = other.First();

				var newProbability = new BiomeProbability { 
					Biome = probability.Biome,
					Probability = new InterpolatingAnimationCurve(probability.Probability, otherBiome.Probability, weight)
				};
				retVal.BiomeProbability.Add(newProbability);
			} else {
				retVal.BiomeProbability.Add(probability);
			}
		}

		foreach (var biomeProbability in second.BiomeProbability) {
			if (retVal.BiomeProbability.Any(b => b.Biome == biomeProbability.Biome)) continue;
			retVal.BiomeProbability.Add(biomeProbability);
		}

		return retVal;
	}
}