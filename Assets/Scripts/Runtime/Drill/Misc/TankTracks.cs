using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TankTracks : MonoBehaviour {

	private DrillController drillController;

	private float trackSpeed = 0;
	private float totalTrackLenght = 0;

	[SerializeField]
	private List<GameObject> trackPrefabs = new List<GameObject>();
	private List<SplineAnimate> tracks = new List<SplineAnimate>();

	[SerializeField]
	private float amount = 30;
	private float offsetToAdd = 0;
	
	private Animator wheelAnimator;
	[SerializeField]
	private AnimationClip wheelClip;

	private void Start() {
		drillController = GetComponentInParent<DrillController>();
		wheelAnimator = GetComponent<Animator>();
		
		spawnTankTracks();

		adjustSpeed();
	}

	private void Update() {
		if (trackSpeed != drillController.GetSpeed()) {
			adjustSpeed();
		}
	}

	private void adjustSpeed() {
		trackSpeed = drillController.GetSpeed();

		var duration = wheelClip.length;
		wheelAnimator.SetFloat("Speed", trackSpeed);

		foreach (var track in tracks) {
			track.Duration = totalTrackLenght / trackSpeed;
		}
	}

	private void spawnTankTracks() {
		offsetToAdd = 1 / amount;

		foreach (var trackPrefab in trackPrefabs) {
			var count = 0;
			var startOffset = 0f;

			tracks.Add(trackPrefab.GetComponent<SplineAnimate>());
			totalTrackLenght = trackPrefab.GetComponent<SplineAnimate>().Container.CalculateLength();

			while (count < amount) {
				count++;
				startOffset += offsetToAdd;

				var track = Instantiate(trackPrefab);
				track.transform.parent = gameObject.transform;

				var spline = track.GetComponent<SplineAnimate>();
				spline.StartOffset = startOffset;

				tracks.Add(spline);
			}
		}
	}

	public void StartTracks() {
		wheelAnimator.SetTrigger("Start");
		foreach (var track in tracks) {
			track.Play();
		}
	}

	public void StopTracks() {
		wheelAnimator.SetTrigger("Stop");
		foreach (var track in tracks) {
			track.Pause();
		}
	}
}
