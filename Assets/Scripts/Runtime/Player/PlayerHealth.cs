using UnityEngine;
using UnityEngine.Events;


public class PlayerHealth : MonoBehaviour {
    private PlayerData playerData;

    public float Health;
	public float MaxHealth { get => playerData.MaxHealth; set => playerData.MaxHealth = value; }

	public UnityEvent OnHealthDepleted;
	private bool deathTriggered = false;

	private void Start() {
        playerData = GetComponent<PlayerData>();
		deathTriggered = false;
	}

	private void CheckIfPlayerIsDead() {
		if (Health <= 0 && !deathTriggered) {
			deathTriggered = true;
			if(OnHealthDepleted != null) OnHealthDepleted.Invoke();
		}
	}

	public void TakeDamage(float damage) {
        Health -= damage;
		CheckIfPlayerIsDead();
		Health = Mathf.Clamp(Health, 0, MaxHealth);
	}

	public void Heal(float healAmount) {
        Health += healAmount;
		Health = Mathf.Clamp(Health, 0, MaxHealth);
	}

#if UNITY_EDITOR
	private void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 250, 700, 700));
        GUILayout.Label("Health: " + Health);
        GUILayout.EndArea();
    }
#endif

}
