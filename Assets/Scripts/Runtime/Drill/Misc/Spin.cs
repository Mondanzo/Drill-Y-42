using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

	public bool RotateOppositeDirection = false;
	public bool useUnscaledTime = false;
	public Vector3 axis = Vector3.zero;

	public float targetRotationSpeed;
	public float acceleration;

	public bool accelerate = true;

	public float currentRotationSpeed = 0;

	private Vector3 axisToUse;
	public Transform target;

	private void Start() {
		if (target == null) {
			target = transform;
		}

		if (axis == Vector3.zero)
			axisToUse = target.forward;
		else
			axisToUse = axis;
	}

	// Update is called once per frame
	void Update() {

		if (accelerate && currentRotationSpeed < targetRotationSpeed) {
			currentRotationSpeed += acceleration * Time.deltaTime;
		} else if (currentRotationSpeed > 0) {
			currentRotationSpeed = Mathf.Max(currentRotationSpeed - acceleration * Time.deltaTime, 0);
		}

		var rotationSpeed = RotateOppositeDirection ? (-currentRotationSpeed) : currentRotationSpeed;
		target.Rotate(axisToUse, rotationSpeed * (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
	}
}