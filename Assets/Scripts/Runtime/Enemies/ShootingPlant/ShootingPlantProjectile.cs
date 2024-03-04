using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPlantProjectile : MonoBehaviour {
    private float launchVelocity = 200;

    public void Setup(float launchForce) {
        launchVelocity = launchForce;
    }

    private void Start() {
        gameObject.GetComponentInChildren<Rigidbody>().AddRelativeForce(Vector3.forward * launchVelocity);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            print("player was hit by projectile");
            // TODO: do damage to player
        }
        // TODO: add vfx and sfx
        Destroy(gameObject, 0.2f);
    }
}
