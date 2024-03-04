using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerZone : MonoBehaviour {
    public bool OneShot = false;
    private bool oneShotEnter, oneShotStay, oneShotExit = false;

    public UnityEvent PlayerEntered;
    public UnityEvent PlayerStay;
    public UnityEvent PlayerExited;


    public void Destroy(GameObject obj) {
        GameObject.Destroy(obj);
    }

    private void OnTriggerEnter(Collider other) {
        if (oneShotEnter) return;

        if (other.CompareTag("Player")) {
            PlayerEntered.Invoke();
            oneShotEnter = OneShot;
        }
    }
    private void OnTriggerStay(Collider other) {
        if (oneShotStay) return;

        if (other.CompareTag("Player")) {
            PlayerStay.Invoke();
            oneShotStay = OneShot;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (oneShotExit) return;

        if (other.CompareTag("Player")) {
            PlayerExited.Invoke();
            oneShotExit = OneShot;
        }
    }
}
