using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlueprintUI : MonoBehaviour
{
    [SerializeField]
    private GameObject bluePrintUICanvas;
    [SerializeField]
    private TextMeshProUGUI upgradeNameTextMesh;

    void Start()
    {
        bluePrintUICanvas.SetActive(false);
    }

    public void Show(string upgradeName) {
        StopAllCoroutines();

        upgradeNameTextMesh.text = upgradeName;
        bluePrintUICanvas.SetActive(true);

        StartCoroutine(hideAfterSeconds(5));
    }

    private IEnumerator hideAfterSeconds(int secs) {
        yield return new WaitForSeconds(secs);
        bluePrintUICanvas.SetActive(false);
    }
}
