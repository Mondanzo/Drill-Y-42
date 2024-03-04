using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomOre : MonoBehaviour {

    public List<Ores> PossibleOres;
    public MinMaxFloat RandomSize = new MinMaxFloat(0.5f, 2f, 0.8f, 1.1f);
    public MinMaxFloat RandomRotate = new MinMaxFloat(0.00f, 5f, 0.00f, 2f);

    [SerializeReference]
    [HideInInspector]
    private Ores placedOre;

    [ContextMenu("Place Ore")]
    public void PlaceOre() {
        PlaceOre(GetRandomOre());
    }
    
    private Bounds GetLocalBoundingBox() {
        Bounds retVal = new Bounds();

        foreach (var ore in PossibleOres) {
            foreach (var rend in ore.GetComponentsInChildren<Renderer>()) {
                var lossyScale = rend.transform.lossyScale;

                var box = rend.localBounds;
                box.size = transform.rotation * Quaternion.Inverse(rend.transform.rotation) * Vector3.Scale(box.size, lossyScale);
                box.center = Vector3.Scale(box.center, lossyScale);
                box.center += transform.InverseTransformPoint(rend.transform.position);
                retVal.Encapsulate(box);
            }
        }

        return retVal;
    }

    public void PlaceOre(Ores ore) {
        if (placedOre) {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying) {
                Destroy(placedOre.gameObject);
            }
            else {
                DestroyImmediate(placedOre.gameObject);
            }
            #else
            Destroy(placedOre.gameObject);
            #endif
        }

        if (PossibleOres.Count <= 0) return;
        
        if (ore == null) return;
        var localTransform = transform;
        placedOre = Instantiate(ore, localTransform.position + ore.transform.localPosition, localTransform.rotation * ore.transform.localRotation, localTransform);
        placedOre.transform.localScale *= RandomSize.GetInRange();
        placedOre.transform.localRotation *= GetRandomRotation();
    }

	
    private Quaternion GetRandomRotation() {
        return Quaternion.Euler(0, RandomRotate.GetInRange(), 0);
    }
	
	
    private Ores GetRandomOre() {

        return PossibleOres[Random.Range(0, PossibleOres.Count)];
    }

    // EDITOR FUNCTIONALITIES
    #if UNITY_EDITOR

    public void Reset() {
        if (placedOre) {
            DestroyImmediate(placedOre);
        }
    }
    
    public void OnDrawGizmosSelected() {
        foreach (var ore in PossibleOres) {
            Gizmos.color = Color.magenta;

            foreach (var mesh in ore.GetComponentsInChildren<MeshFilter>()) {
                Gizmos.matrix = transform.localToWorldMatrix * mesh.transform.localToWorldMatrix;
                Gizmos.DrawWireMesh(mesh.sharedMesh);
            }
        }
    }

    public void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "RandomOre.png");
        if(PossibleOres.Count > 0) {
            var ore = PossibleOres[0];
            Gizmos.color = isActiveAndEnabled ? Color.green : Color.red;
            foreach (var mesh in ore.GetComponentsInChildren<MeshFilter>()) {
                Gizmos.matrix = transform.localToWorldMatrix * mesh.transform.localToWorldMatrix;
                Gizmos.DrawWireMesh(mesh.sharedMesh);
            }
        }
    }
    #endif
}