using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class DrillController : MonoBehaviour {

	[Header("Settings")]
	[SerializeField]
	private float boostSpeed = 5;
	private float speed = 1f;
	private int gear = 1;

	[SerializeField]
	private float fuel = 50;
	public float Fuel {
		get => fuel;
		set {
			fuel = value;
			if (fuel <= 0) {
				if (onDrillEmpty != null) onDrillEmpty.Invoke();
			}
		}
	}

	public float MaxFuel { get => drillData.MaxFuel; set => drillData.MaxFuel = value; }
	private float fuelConsumption = 1;

	private DrillData drillData;
	private StudioEventEmitter drillSound;
	private List<Spin> spins;
	private TankTracks tracks;

	private bool drillHalted = true;
	public bool DrillHalted {
		get => drillHalted;
		set {
			if (drillHalted != value) {
				drillHalted = value;
				if (drillHalted) {
					if (OnDrillHalted != null) OnDrillHalted.Invoke();
				} else {
					if (OnDrillContinue != null) OnDrillContinue.Invoke();
				}
			}
		}
	}

	public float drillDistance;

	[Header("Events")]
	public UnityEvent onDrillEmpty;
	private bool isDrillNotRepaired = true;
	public UnityEvent OnDrillRepairedFirstTime;
	private bool isNeverRefueled = true;
	public UnityEvent OnDrillRefueledFirstTime;

	public UnityEvent OnDrillHalted;
	public UnityEvent OnDrillContinue;

	private void Start() {
		drillData = GetComponent<DrillData>();
		drillSound = GetComponent<StudioEventEmitter>();
		spins = GetComponents<Spin>().ToList();
		tracks = GetComponentInChildren<TankTracks>();
	}

	private void FixedUpdate() {
		checkDrillRepair();

		setSpeed();
		setFuelConsumption();

		if (hasEnoughFuel() && !DrillHalted) {
			moveForward();
			consumeFuel();
			drillRunning();

			drillDistance += Time.deltaTime * speed;
		} else {
			drillStopped();
		}
	}

	private void setFuelConsumption() {
		fuelConsumption = drillData.BaseFuelConsumption * gear;
	}

	private void setSpeed() {
		speed = drillData.BaseSpeed * gear;
	}

	private void drillStopped() {
		drillSound.Stop();
		tracks.StopTracks();
		SetDrillHead(false);
	}

	private void drillRunning() {
		if (!drillSound.IsPlaying()) drillSound.Play();
		tracks.StartTracks();
		SetDrillHead(true);
	}

	private bool checkDrillRepair() {
		if (drillData.IsDrillRepaired == false) {
			SetDrillHalt(true);
			return false;
		}

		// only call this code once up the first time the drill is repaired
		if (isDrillNotRepaired) {
			if (OnDrillRepairedFirstTime != null) OnDrillRepairedFirstTime.Invoke();
			isDrillNotRepaired = false;
		}

		return true;
	}

	private void SetDrillHead(bool newState) {
		foreach (var spin in spins) {
			spin.accelerate = newState;
		}
	}

	private bool hasEnoughFuel() {
		return Fuel > 0;
	}

	private void consumeFuel() {
		Fuel -= fuelConsumption * Time.deltaTime;
		Fuel = Mathf.Clamp(Fuel, 0, MaxFuel);
	}

	private void moveForward() {
		transform.Translate(transform.forward * -1 * Time.deltaTime * speed);
	}



	public float GetSpeed() {
		return speed;
	}

	public void SetDrillHalt(bool newState) {
		DrillHalted = newState;
	}

	public void AddFuel(float newFuel) {
		if (isNeverRefueled) {
			if (OnDrillRefueledFirstTime != null) OnDrillRefueledFirstTime.Invoke();
			isNeverRefueled = false;
		}

		Fuel += newFuel;
		Fuel = Mathf.Clamp(Fuel, 0, MaxFuel);
	}

	public void StartBoost() {
		gear = Mathf.RoundToInt(boostSpeed * drillData.MaxGear);
		drillSound.EventInstance.setParameterByNameWithLabel("DrillSpeed", "Fast");
	}

	public void StopBoost() {
		gear = 1;
		drillSound.EventInstance.setParameterByNameWithLabel("DrillSpeed", "Normal");
	}

#if UNITY_EDITOR
	private void OnGUI() {
		GUILayout.BeginArea(new Rect(0, 300, 700, 700));
		GUILayout.Label("Drill Fuel: " + Fuel + " Fuel Consumption Per Second: " + fuelConsumption);
		GUILayout.Label("Gear Level: " + gear + " Actual Speed: " + speed);
		GUILayout.Label("Drill Distance: " + drillDistance);
		GUILayout.EndArea();
	}
#endif
}