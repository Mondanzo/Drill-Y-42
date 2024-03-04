using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DistanceBarBehavior : MonoBehaviour
{
    public GameObject player;
    public GameObject drill;

    [SerializeField]
    private GameObject Tunnel;

    [SerializeField]
    private UIDocument m_UIDocument;

    private float distance;
    private VisualElement playerElement;    
    private Label collapsingWallMeterElement;
    private Label destructibleWallMeterElement;
    private DrillTunnelGenerator tunnelGenerator;

    void Start()
    {
        var rootElement = m_UIDocument.rootVisualElement;
        playerElement = rootElement.Q<VisualElement>("Player");
        collapsingWallMeterElement = rootElement.Q<Label>("CollapsingWallMeter");
        destructibleWallMeterElement = rootElement.Q<Label>("DestructibleWallMeter");

        tunnelGenerator = Tunnel.GetComponent<DrillTunnelGenerator>();
    }

    void Update()
    {
        var rootElement = m_UIDocument.rootVisualElement;
        playerElement = rootElement.Q<VisualElement>("Player");
        collapsingWallMeterElement = rootElement.Q<Label>("CollapsingWallMeter");
        destructibleWallMeterElement = rootElement.Q<Label>("DestructibleWallMeter");

        distance =
            (Vector3.Distance(
                new Vector3(0, 0, player.transform.position.z),
                tunnelGenerator.GetNextDestructibleWallPoint()
            ) + 50)
            -
            Vector3.Distance(
                new Vector3(0, 0, player.transform.position.z),
                tunnelGenerator.GetNextCollapsingWallPoint()
            );
        var calculatedDistance = Mathf.Clamp(476 - (distance * 2.35f), 0, 476 - 10f);

        collapsingWallMeterElement.text = Mathf.Floor(Mathf.Clamp(drill.GetComponent<DrillController>().drillDistance - 115, 0, Mathf.Infinity)).ToString() + "m";
        destructibleWallMeterElement.text = Mathf.Floor(drill.GetComponent<DrillController>().drillDistance).ToString() + "m";  
        
        playerElement.style.left = Mathf.Lerp(playerElement.style.left.value.value, calculatedDistance, 0.1f); 
    }
}
