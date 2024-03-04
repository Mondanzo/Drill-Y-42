using UnityEngine;

public class LookAtTutorial : MonoBehaviour {
	public void Update() {
		if(Camera.main) transform.LookAt(transform.position + Camera.main.transform.forward);
	}
}