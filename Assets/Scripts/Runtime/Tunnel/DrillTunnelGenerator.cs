using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
#if UNITY_EDITOR
using UnityEditor;
#endif


[AddComponentMenu("Tunnels/Drill Tunnel Generator")]
[RequireComponent(typeof(MoveBack))]
public class DrillTunnelGenerator : MonoBehaviour {


	[Header("Generation Segments")]
	public List<DrillTunnelSegment> generateableSegments;
	public List<DrillTunnelSegment> safeCaves;
	
	public int safeCaveCooldown = 10;

	[CustomSeparator]
	
	[Header("Cave Generation Settings")]
	public List<CaveGenerationSettings> possibleSettings;
	public int segmentsTilNext = 8;
	private int currentSegmentsLeft = 0;

	[CustomSeparator]
	
	[Header("Destructible Wall Settings")]
	public DestructibleWall DestructibleWall;
	public int dstructibleWallsPoolSize = 2;
	public float destructibleOffset = 30;
	
	[NonSerialized]
	public float totalCurrentLength = 0;

	[CustomSeparator]
	
	[Header("Collapsing Wall Settings")]
	public CollapsingWall CollapsingWall;

	public AnimationCurve collapsingWallSpeedModifierBasedOnDistanceToPlayer = AnimationCurve.Linear(50, 1, 100, 2);
	public float collapsableOffset = 0;
	public float collapsingWallMetersPerSecond = 0.01f;
	public bool doCollapse = false;

	[CustomSeparator] public float moveBackThreshhold = 500;
	
	private Vector3 nextSpawnPoint;
	
	public int NextSafeCaveCooldown;

	private Queue<DrillTunnelSegment> generatedSegments;
	
	
	private CollapsingWall collapsingWallInstance;
	private float collapsingModifier;
	
	
	private Queue<DestructibleWall> destructibleWallsPool;

	private CaveGenerationSettings currentSettings;
	private CaveGenerationSettings nextSettings;
	
	// an event for when SetCollapsing is called, delgate
	public delegate void CollapsingStateChanged(bool collapse);
	public event CollapsingStateChanged OnCollapsingStateChanged;

	public void SetCollapsing(bool collapse) {
		doCollapse = collapse;
		collapsingWallInstance.SetDeadly(doCollapse);
		
		OnCollapsingStateChanged?.Invoke(collapse);
	}
	

	public void Start() {
		generatedSegments = new Queue<DrillTunnelSegment>();
		destructibleWallsPool = new Queue<DestructibleWall>(dstructibleWallsPoolSize);
		nextSettings = possibleSettings[Random.Range(0, possibleSettings.Count)];

		DoCurrentSegmentsLeftCheck();

		FillQueueWithPreGenerated();

		collapsingWallInstance = Instantiate(CollapsingWall, GetNextCollapsingWallPoint(), transform.rotation, transform);

		for (int i = 0; i < dstructibleWallsPoolSize; i++) {
			var wall = Instantiate(DestructibleWall, GetNextDestructibleWallPoint(), transform.rotation, transform);
			wall.WallDestroyed += sender => { MoveDestructibleWall(); };
			destructibleWallsPool.Enqueue(wall);

			MoveWalls();
		}
	}


	private void FillQueueWithPreGenerated() {
		totalCurrentLength = 0;

		foreach (var segment in transform.GetComponentsInChildren<DrillTunnelSegment>()) {
			safeCaveCooldown--;
			currentSegmentsLeft--;
			CaveGenerator generator;

			if (segment.TryGetComponent(out generator)) {
				generator.generationSettings = currentSettings;
			}
			
			generatedSegments.Enqueue(segment);
			nextSpawnPoint = segment.GetEndPosition();
			totalCurrentLength += segment.GetDistance();
		}
	}


	public Vector3 GetNextCollapsingWallPoint() {
		if (generatedSegments.Count < 0) return collapsingWallInstance.transform.position;

		CleanUpIfPossible();

		float offset = collapsableOffset;

		int index = GetSegmentIndexByDistance(offset);

		DrillTunnelSegment segment;

		if (index < 0) {
			var tSegment = generatedSegments.Peek();
			if (tSegment == null) return Vector3.down * 100;
			var tPos = tSegment.GetStartPosition() + Vector3.forward * offset;
			return tPos;
		} 
		
		segment = generatedSegments.ElementAt(index);


		var segmentProgress = offset - segment.GetStartPointOffset();
		
		if (segment.haltCollapsingWall && segmentProgress >= segment.haltingOffset) {
			segment.haltCollapsingWall = false;
			StopCollapsing();
		}
		
		return Vector3.Lerp(segment.GetStartPosition(), segment.GetEndPosition(), segmentProgress / segment.GetDistance());
	}

	private void StopCollapsing() {
		SetCollapsing(false);
	}
	
	
	private void MoveBack() {
		var l = new List<Transform>();

		foreach (var segment in generatedSegments) {
			l.Add(segment.transform);
		}

		l.Add(collapsingWallInstance.transform);

		foreach (var wall in destructibleWallsPool) {
			l.Add(wall.transform);
		}

		var lengthToMoveBack = generatedSegments.Peek().GetStartPointOffset();

		nextSpawnPoint += Vector3.back * lengthToMoveBack;

		GetComponent<MoveBack>().MoveBackByAmount(lengthToMoveBack, l);
		collapsableOffset -= lengthToMoveBack;
		destructibleOffset -= lengthToMoveBack;
		totalCurrentLength -= lengthToMoveBack;
	}

	/*
	 * Returns the index of the segment or -1 if none is found.
	 */
	public int GetSegmentIndexByDistance(float distance) {

		for (int i = 0; i < generatedSegments.Count; i++) {
			var segment = generatedSegments.ElementAt(i);

			if (segment.GetStartPointOffset() <= distance && segment.GetEndPointOffset() > distance) {
				return i;
			}
		}
		
		return -1;
	}

	public Vector3 GetNextDestructibleWallPoint(bool generateIfMissing = false, int safeLimit = 100) {
		if (generatedSegments.Count < 0) return transform.position;

		if (safeLimit <= 0) {
			Debug.LogError("Infinite loop! Something is not right.");
			return Vector3.zero;
		}

		int index = GetSegmentIndexByDistance(destructibleOffset);

		DrillTunnelSegment segment;

		if (index < 0) {
			if(generateIfMissing) {
				GenerateNextSegment();
				return GetNextDestructibleWallPoint(true, safeLimit - 1);
			} else {
				return generatedSegments.Last().GetEndPosition();
			}
		}

		segment = generatedSegments.ElementAt(index);

		if (segment.openWalls) {
			if(generateIfMissing) {
				destructibleOffset += segment.GetDistance() + segment.openWallsDestructibleOffset;
				return GetNextDestructibleWallPoint(true, safeLimit - 1);
			} else {
				return segment.GetEndPosition();
			}
		}

		var segmentProgress = destructibleOffset - segment.GetStartPointOffset();
		return Vector3.Lerp(segment.startPosition, segment.endPosition, segmentProgress / segment.GetDistance()) + segment.transform.position;
	}

	public void CleanUpIfPossible() {
		#if UNITY_EDITOR
		if (!EditorApplication.isPlaying) return;
		#endif

		if (GetSegmentIndexByDistance(collapsableOffset) < 1) return;

		var segment = generatedSegments.Dequeue();

		Destroy(segment.gameObject);
	}

	public void MoveWalls() {
		MoveCollapsingWall();
		MoveDestructibleWall();
	}

	public void MoveDestructibleWall() {
		var wall = destructibleWallsPool.Dequeue();
		wall.transform.position = GetNextDestructibleWallPoint(true);
		destructibleOffset += DestructibleWall.GetDistance();
		destructibleWallsPool.Enqueue(wall);
	}

	public void MoveCollapsingWall() {
		var newPos = GetNextCollapsingWallPoint();
		if (float.IsNaN(newPos.z)) return;

		collapsingWallInstance.transform.position = newPos;
		collapsableOffset += (collapsingWallMetersPerSecond * collapsingModifier) / Time.deltaTime;
	}


	public void TeleportCollapsingWall(DrillTunnelSegment segmentOffset) {
		collapsableOffset = segmentOffset.GetEndPointOffset() - collapsingWallInstance.GetDistance() + segmentOffset.spawnOffset;
		MoveCollapsingWall();
	}

	public void Update() {
		if (PauseScreen.isPaused) return;
		if (doCollapse) {
			CheckDistanceToPlayer();
			MoveCollapsingWall();

		}
		
		if(totalCurrentLength >= moveBackThreshhold) MoveBack();
	}

	
	private void CheckDistanceToPlayer() {
		var drill = FindObjectOfType<DrillController>();
		collapsingModifier = Mathf.Max(collapsingWallSpeedModifierBasedOnDistanceToPlayer.Evaluate(Mathf.Abs(drill.transform.position.z - collapsableOffset)), 0);
	}

	
	DrillTunnelSegment GenerateNextSegment() {
		int idx;
		DrillTunnelSegment segment;

		if (safeCaveCooldown <= 0) {
			idx = Random.Range(0, safeCaves.Count);
			segment = safeCaves[idx];
			safeCaveCooldown = NextSafeCaveCooldown + 1;
		} else {
			idx = Random.Range(0, generateableSegments.Count);
			segment = generateableSegments[idx];
		}

		safeCaveCooldown--;

		GameObject instantiatedSegment;

		#if UNITY_EDITOR
		if (!EditorApplication.isPlaying) {
			instantiatedSegment = (GameObject) PrefabUtility.InstantiatePrefab(segment.gameObject, transform);
		} else {
			instantiatedSegment = Instantiate(segment.gameObject, transform);
		}
		#else
		instantiatedSegment = Instantiate(segment.gameObject, transform);
		#endif
		
		CaveGenerator generator;

		if (instantiatedSegment.TryGetComponent(out generator)) {
			generator.generationSettings = currentSettings.InterpolateSettings(nextSettings, (float) currentSegmentsLeft / segmentsTilNext);
		}

		DoCurrentSegmentsLeftCheck();

		var drilLSegmentComponent = instantiatedSegment.GetComponent<DrillTunnelSegment>();
		nextSpawnPoint = drilLSegmentComponent.Stitch(nextSpawnPoint);

		CleanUpIfPossible();
		if (generatedSegments != null) generatedSegments.Enqueue(drilLSegmentComponent);
		totalCurrentLength += segment.GetDistance();
		return drilLSegmentComponent;
	}
	
	
	private void DoCurrentSegmentsLeftCheck() {
		currentSegmentsLeft--;

		if (currentSegmentsLeft <= 0) {
			currentSegmentsLeft = segmentsTilNext;
			currentSettings = nextSettings;
			nextSettings = possibleSettings[Random.Range(0, possibleSettings.Count)];
		}
	}

	public void Clear() {
		int count = transform.childCount;

		for (int i = 0; i < count; i++) {
			DestroyImmediate(transform.GetChild(0).gameObject);
		}

		safeCaveCooldown = NextSafeCaveCooldown;
		nextSpawnPoint = transform.position;
	}

	public void Generate(int segmentCount, bool clear = false) {
		NextSafeCaveCooldown = safeCaveCooldown;
		
		if (clear) {
			Clear();
		}


		for (int i = 0; i < segmentCount; i++) {
			GenerateNextSegment();
		}
	}

	private void OnDrawGizmosSelected() {
		if (DestructibleWall) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube(transform.position + destructibleOffset * Vector3.forward + DestructibleWall.GetCenter(), DestructibleWall.GetSize());
		}

		if (CollapsingWall) {
			Gizmos.color = Color.blue;
			Gizmos.DrawCube(transform.position + collapsableOffset * Vector3.forward + CollapsingWall.GetCenter(), CollapsingWall.GetSize());
		}
	}
}