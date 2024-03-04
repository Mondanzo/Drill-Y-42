using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPlantController : MonoBehaviour {
    [SerializeField]
    private Transform projectileSpawnPoint;

    private Animator animator;

    private Transform target;

    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float projectileLaunchForce = 4000;


    void Start() {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate() {
        if (target) {
            projectileSpawnPoint.LookAt(target.position);

            RaycastHit hit;
            var wasHit = Physics.Raycast(projectileSpawnPoint.position, projectileSpawnPoint.forward, out hit);
            Debug.DrawRay(projectileSpawnPoint.position, projectileSpawnPoint.forward * hit.distance, Color.yellow);

            if (wasHit) {
                if (hit.collider.gameObject.tag == "Player") {
                    animator.SetBool("isOpen", true);
                }
                else {

                }
            }
        }

    }

    public void OnPlantOpened() {
        var projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint);
        projectileInstance.GetComponent<ShootingPlantProjectile>().Setup(projectileLaunchForce);
        projectileInstance.transform.parent = null;
    }

    public void OnPlantClosed() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            target = null;
            animator.SetBool("isOpen", false);
        }
    }
}
