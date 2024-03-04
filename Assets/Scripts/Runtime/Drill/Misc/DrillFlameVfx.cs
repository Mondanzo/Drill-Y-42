using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DrillFlameVfx : MonoBehaviour {

	[Header("Settings")]
	[SerializeField]
	private float duration = 2f;

	private VisualEffect visualEffect;
	private DrillController drillController;

	private void Start() {
		visualEffect = GetComponent<VisualEffect>();
		drillController = GetComponentInParent<DrillController>();
		drillController.OnDrillContinue.AddListener(startVfx);
	}

	private void startVfx() {
		visualEffect.Play();
		StartCoroutine(stop());
	}

	private IEnumerator stop() {
		yield return new WaitForSeconds(duration);
		visualEffect.Stop();
	}
}
