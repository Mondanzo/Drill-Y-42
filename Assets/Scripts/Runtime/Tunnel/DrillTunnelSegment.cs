using UnityEngine;

[AddComponentMenu("Tunnels/Drill Tunnel Segment")]
public class DrillTunnelSegment : MonoBehaviour {

	public bool openWalls;
	public bool haltCollapsingWall = false;

	public Vector3 startPosition;
	public Vector3 endPosition;

	[Header("Collapsing wall - Yellow box")]
	public float spawnOffset;
	
	[Header("Destructible Wall - Blue Box")]
	public float openWallsDestructibleOffset;

	[Header("Collapsing Wall - White Box")]
	public float haltingOffset;

	public float GetDistance() {
		return (endPosition - startPosition).magnitude;
	}


	public float GetStartPointOffset() {
		return transform.position.z + startPosition.z;
	}

	
	public float GetEndPointOffset() {
		return transform.position.z + endPosition.z;
	}

	
	public Vector3 GetStartPosition() {
		return transform.position + startPosition;
	}


	public Vector3 GetEndPosition() {
		return transform.position + endPosition;
	}

	
	// Returns the next startPoint
	public Vector3 Stitch(Vector3 nextSpawnPoint) {
		transform.position = nextSpawnPoint - startPosition;
		return transform.position + endPosition;
	}

	public void TeleportCollapsingWall() {
		var generator = GetComponentInParent<DrillTunnelGenerator>();
		generator.TeleportCollapsingWall(this);
		generator.SetCollapsing(true);
	}
	
	
	#region Gizmos
	
	public void OnDrawGizmos() {
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.color = Color.green;
		Gizmos.DrawCube(startPosition + Vector3.forward * 0.25f, new Vector3(1, 1, 0.5f));
		
		Gizmos.color = Color.red;
		Gizmos.DrawCube(endPosition - Vector3.forward * 0.25f, new Vector3(1, 1, 0.5f));
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(endPosition + Vector3.forward * spawnOffset, new Vector3(6, 0.5f, 0.5f));
		
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(endPosition + Vector3.forward * openWallsDestructibleOffset, new Vector3(6, 0.5f, 0.5f));

		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(startPosition + Vector3.forward * haltingOffset, new Vector3(6, 0.5f, 0.5f));
	}
	
	#endregion
}