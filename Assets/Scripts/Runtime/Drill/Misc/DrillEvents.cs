using UnityEngine;
using UnityEngine.Events;

public class DrillEvents : MonoBehaviour {
	private DrillController drillController;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onDrillEmpty;
	[SerializeField]
	private UnityEvent onDrillRepairedFirstTime;
	[SerializeField]
	private UnityEvent onDrillRefueledFirstTime;
	[SerializeField]
	private UnityEvent onDrillHalted;
	[SerializeField]
	private UnityEvent onDrillContinue;


	private void Start() {
		drillController = GetComponentInParent<DrillController>();
		drillController.onDrillEmpty.AddListener(() => { if (onDrillEmpty != null) onDrillEmpty.Invoke(); });
		drillController.OnDrillRepairedFirstTime.AddListener(() => { if (onDrillRepairedFirstTime != null) onDrillRepairedFirstTime.Invoke(); });
		drillController.OnDrillRefueledFirstTime.AddListener(() => { if (onDrillRefueledFirstTime != null) onDrillRefueledFirstTime.Invoke(); });
		drillController.OnDrillHalted.AddListener(() => { if (onDrillHalted != null) onDrillHalted.Invoke(); });
		drillController.OnDrillContinue.AddListener(() => { if (onDrillContinue != null) onDrillContinue.Invoke(); });
	}
}
