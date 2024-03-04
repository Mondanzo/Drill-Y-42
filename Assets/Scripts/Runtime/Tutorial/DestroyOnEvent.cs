using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEvent : MonoBehaviour {
    public MonoBehaviour destroyComponent;
    public GameObject destroyGameObject;

    public void DestroyComponent() {
        Destroy(getComponent());
    }

    private MonoBehaviour getComponent() {
        if (destroyComponent != null) return destroyComponent;
        return destroyComponent;
    }

    public void DestroyGameObject() {
        Destroy(GetGO());
    }

    private GameObject GetGO() {
        if (destroyGameObject != null) return destroyGameObject;
        return destroyGameObject;
    }
}
