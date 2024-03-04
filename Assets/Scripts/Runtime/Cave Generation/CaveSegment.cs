using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
#if UNITY_EDITOR
//using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;
using Color = UnityEngine.Color;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class CaveSegment : MonoBehaviour {

	[SerializeField] public List<LinkPoint> linkPoints = new List<LinkPoint>();

	public bool forcedEnd;

	public float SegmentValue = 1;

	[Range(0, 1)] public float Rarity = 1f;

	public CaveSegment previous;
	public List<CaveSegment> nexts;

	public CaveGenerationSettings.SerializedGenerationSettings usedSettings;
	
	#if UNITY_EDITOR
	[SerializeField] public bool showDebugBoundingBoxes = false;
	#endif

	public Bounds GetBoundingBox() {
		Bounds retVal = new Bounds(transform.position, Vector3.zero);
		foreach (var rend in GetComponentsInChildren<Renderer>()) {
			retVal.Encapsulate(rend.bounds);
		}

		return retVal;
	}


	private Bounds cachedBounds;

	public Bounds GetLocalBoundingBox() {
		Bounds retVal = new Bounds();
		if (cachedBounds.size.sqrMagnitude > 0) return cachedBounds;
		
		foreach (var rend in GetComponentsInChildren<Renderer>()) {
			var lossyScale = rend.transform.lossyScale;

			var box = rend.localBounds;
			box.size = transform.rotation * Quaternion.Inverse(rend.transform.rotation) * Vector3.Scale(box.size, lossyScale);
			box.center = Vector3.Scale(box.center, lossyScale);
			box.center += transform.InverseTransformPoint(rend.transform.position);
			retVal.Encapsulate(box);
		}

		cachedBounds = retVal;
		return retVal;
	}


	public List<Collider> GetElementsInSegmentByTag(string tag) {
		return Physics.OverlapBox(transform.position, GetLocalBoundingBox().extents, transform.rotation).Where(h => h.CompareTag(tag)).ToList();
	}

	
	public GameObject GetPlayerInSegment() {
		Collider[] hits = new Collider[1];
		Physics.OverlapBoxNonAlloc(transform.position, GetLocalBoundingBox().extents, hits, transform.rotation, LayerMask.GetMask("Player"));

		if (hits[0] != null) {
			return hits[0].gameObject;
		}

		return null;
	}
	

	private CaveGenerator GetGenerator() {
		if (this is CaveGenerator generator) return generator;
		if (previous != null) return previous.GetGenerator();
		return null;
	}

	public float GetCurrentDepthPercentage() {
		return 1 - usedSettings.GenerationCurrency / GetGenerator().usedSettings.GenerationCurrency;
	}
	

	public List<CaveSegment> LinkNext(CaveGenerationSettings.SerializedGenerationSettings settings, Transform root) {
		usedSettings = settings;
		var generatedSegments = new List<CaveSegment>();
		var attachmentPoints = GetAttachmentPoints();
		ShuffleList(ref attachmentPoints);
		foreach (var localConnectionPoint in attachmentPoints) {
			if (localConnectionPoint.isOccupied) continue;

			var newSegment = LinkSegment(settings, root, localConnectionPoint);
			
			if (newSegment) {
				generatedSegments.Add(newSegment);
				newSegment.previous = this;
			} else {
				// Couldn't find a suitable segment to link.
				var segmentList = FindSegmentsByRequirements(settings.PossibleSegments, -0.5f);
				if(segmentList.Count > 0) {
					var deadEnd = Instantiate(segmentList[0], root);
					var deadEndTransform = deadEnd.transform;
					Vector3 deadEndPos;
					Quaternion deadEndRot = deadEndTransform.rotation;
					
					AlignPositionAndRotation(out deadEndPos, ref deadEndRot, localConnectionPoint, deadEnd.GetAttachmentPoints()[0]);
					
					deadEndTransform.position = deadEndPos;
					deadEndTransform.rotation = deadEndRot;
				}
				forcedEnd = true;
			}
		}
		
		nexts = generatedSegments;
		return generatedSegments;
	}
	
	
	private CaveSegment LinkSegment(CaveGenerationSettings.SerializedGenerationSettings settings, Transform root, LinkPoint localConnectionPoint) {
		var availableNext = FindSegmentsByRequirements(settings.PossibleSegments, Mathf.Max(0, settings.GenerationCurrency)); // must be shuffled
		if (availableNext.Count <= 0) return null;

		foreach(var next in availableNext) {
			var attachmentsPoints = next.GetAttachmentPoints();
			ShuffleList(ref attachmentsPoints);
			foreach(var otherConnectionPoint in attachmentsPoints) {
				Vector3 nextPos;
				var nextRot = next.transform.rotation;
				
				AlignPositionAndRotation(out nextPos, ref nextRot, localConnectionPoint, otherConnectionPoint);
				
				var generator = GetGenerator();

				var curvatureFine = true;
				
				foreach (var otherOtherPoint in attachmentsPoints) {
					if (otherOtherPoint == otherConnectionPoint) continue;
					var curvature = Vector3.Dot(nextRot * otherOtherPoint.rotation * Vector3.forward, generator.GetAttachmentPoints()[0].rotation * Vector3.forward);
					var allowedCurvature = generator.generationSettings.CurvatureSettings.Evaluate(Mathf.Abs(generator.transform.position.x - localConnectionPoint.position.x));

					if (curvature < allowedCurvature) {
						curvatureFine = false;
						break;
					}
				}
				
				if (curvatureFine) {
					Bounds box = next.GetLocalBoundingBox();

					if (!CheckIfSpaceIsOccupied(box, nextPos, Quaternion.identity)) {
						// Lock Points
						localConnectionPoint.isOccupied = true;
						otherConnectionPoint.isOccupied = true;

						var other = Instantiate(next, root);
						var otherTransform = other.transform;

						otherTransform.position = nextPos;
						otherTransform.rotation = nextRot;

						otherConnectionPoint.isOccupied = false;
						
						return other;
					}
				}
			}
		}
		return null;
	}
	private void AlignPositionAndRotation(out Vector3 origPosition, ref Quaternion origRotation, LinkPoint localConnectionPoint, LinkPoint otherConnectionPoint) {

		var localTransform = transform;

		// Define Rotations in World Space
		var localRotation = localTransform.rotation * localConnectionPoint.rotation;
		var otherRotation = origRotation * otherConnectionPoint.rotation;
		var directedRotation = localRotation * Quaternion.AngleAxis(180, localRotation * Vector3.up);

		// Get relative rotation from otherRotation to directedRotation
		var relativeRotation = otherRotation.eulerAngles.y - directedRotation.eulerAngles.y;

		// Don't know how it works but it works. Rotates things correctly.
		origRotation = Quaternion.Euler(0, -relativeRotation, 0) * origRotation;


		// Define linking point in world space and set linking points onto the same point.
		Vector3 globalLinkingPoint = localTransform.TransformPoint(localConnectionPoint.position);
		origPosition = globalLinkingPoint - origRotation * otherConnectionPoint.position;
	}


	private bool CheckIfSpaceIsOccupied(Bounds box, Vector3 nextPos, Quaternion rotation) {
		box.Expand(new Vector3(2, 2, 2));
		var hits = Physics.OverlapBox(Quaternion.Inverse(rotation) * nextPos, box.extents, rotation, Int32.MaxValue, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
		foreach (var hit in hits) {
			var segmentComponent = hit.transform.GetComponentInParent<CaveSegment>();
			if (segmentComponent != this) {
				return true;
			}
		}
		
		return false;
	}


	public static void ShuffleList<T>(ref List<T> list) {
		for (int i = list.Count - 1; i >= 1; i--) {
			var j = Random.Range(0, list.Count);
			var k = list[j];
			var l = list[i];
			list[i] = k;
			list[j] = l;
		}
	}
	
	
	public static void ShuffleSegmentList(ref List<CaveSegment> list) {
		for (int i = list.Count - 1; i >= 1; i--) {
			var j = Random.Range(0, list.Count);
			var k = list[j];
			var l = list[i];
			list[i] = k;
			list[j] = l;
		}
	}


	private static List<CaveSegment> FindSegmentsByRequirements(List<CaveSegment> possibleNext, float remainingCurrency) {
		var possibleList = (from segment in possibleNext where segment.SegmentValue <= remainingCurrency && segment.GetAttachmentPoints().Count > 1 select segment).ToList();

		possibleList.Sort((segment, caveSegment) => Random.value * segment.Rarity < Random.value * caveSegment.Rarity ? 1 : -1);

		if (possibleList.Count > 0) return possibleList;
		possibleList = (from segment in possibleNext where segment.SegmentValue <= remainingCurrency select segment).ToList();
		possibleList.Sort((segment, caveSegment) => Random.value * segment.Rarity < Random.value * caveSegment.Rarity ? 1 : -1);
		return possibleList;
	}
	

	public List<LinkPoint> GetAttachmentPoints() {
		return linkPoints;
	}

	public List<LinkPoint> GetFreeAttachmentPoints() {
		return linkPoints.Where(point => !point.isOccupied).ToList();
	}


	#region Editor Gizmos

	public void OnDrawGizmos() {
		if (linkPoints != null) {
			foreach (var point in linkPoints) {
				Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.TRS(point.position, point.rotation, Vector3.one);

				Gizmos.color = point.debugColor;
				Gizmos.DrawRay(Vector3.zero, Vector3.forward);

				Gizmos.DrawCube(Vector3.back * 0.125f, new Vector3(0.5f, 0.5f, 0.25f));
			}
		}

		Gizmos.matrix = Matrix4x4.identity;

		#if UNITY_EDITOR
		if (showDebugBoundingBoxes) {
			Gizmos.matrix = Matrix4x4.identity;
			foreach (var renderer in GetComponentsInChildren<Renderer>()) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
			}
			
			foreach (var rend in GetComponentsInChildren<Renderer>()) {
				Gizmos.matrix = Matrix4x4.Rotate(rend.transform.rotation);
				var tBox = rend.localBounds;
				tBox.size = Vector3.Scale(tBox.size, rend.transform.lossyScale);
				tBox.center = Vector3.Scale(tBox.center, rend.transform.lossyScale);
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireCube(transform.position + tBox.center, tBox.size);
			}
		}

		#endif

		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

		Gizmos.color = forcedEnd ? Color.red : Color.white;
		var box = GetLocalBoundingBox();
		Gizmos.DrawWireCube(box.center, box.size);
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(transform.rotation * box.center, 0.1f);
	}

	#endregion

}

[Serializable]
public record LinkPoint {
	public Vector3 position;
	public Quaternion rotation;
	public Color debugColor;
	public bool isOccupied;
}