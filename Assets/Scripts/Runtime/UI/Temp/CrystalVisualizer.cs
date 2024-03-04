using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalVisualizer : MonoBehaviour
{
    // TEMPORÄR WIRD BESSER AUSSEHEN MIT UITOOLKIT
    // TEMPORÄR WIRD BESSER AUSSEHEN MIT UITOOLKIT
    // TEMPORÄR WIRD BESSER AUSSEHEN MIT UITOOLKIT
    // TEMPORÄR WIRD BESSER AUSSEHEN MIT UITOOLKIT
    // TEMPORÄR WIRD BESSER AUSSEHEN MIT UITOOLKIT

    public GameObject text;

    public void setCrystalAmount(int amount)
    {
        text.GetComponent<TMPro.TextMeshProUGUI>().text = amount.ToString();
    }
}
