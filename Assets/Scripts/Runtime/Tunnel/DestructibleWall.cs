using UnityEngine;
using UnityEngine.Events;

public class DestructibleWall : MonoBehaviour {
	
	public event WallDestroyedEvent WallDestroyed;

	public void Destroy() {
		WallDestroyed.Invoke(this);
	}
	
	private float cachedDistance = 0;
    
	[ContextMenu("Refresh Distance")]
    public float GetDistance() {
        if(cachedDistance == 0) {
            cachedDistance = GetComponent<BoxCollider>().size.z;
        }

        return cachedDistance;
    }

	public Vector3 GetSize() {
		return GetComponent<BoxCollider>().size;
	}
	
	public Vector3 GetCenter() {
		return GetComponent<BoxCollider>().center;
	}
}

public delegate void WallDestroyedEvent(object sender);