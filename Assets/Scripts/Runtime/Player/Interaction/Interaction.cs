using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public enum Clicks {Primary, Interaction, Secondary}
    [SerializeField]
	public Clicks InteractionType = Clicks.Primary;
    
    public InteractionEvent Executed;
    public InteractionEvent InputBegan;
    public InteractionEvent InputEnded;
    public List<GameObject> toHighlight = new List<GameObject>();
    
    public void executeInput(InputAction.CallbackContext context, GameObject other) {
        if (context.action.name != InteractionType.ToString()) return;
    
        if (context.phase == InputActionPhase.Performed) {
            Executed.Invoke(other);            
        } else if (context.phase == InputActionPhase.Started) {
            InputBegan.Invoke(other);
        } else if (context.phase == InputActionPhase.Canceled) {
            InputEnded.Invoke(other);
        }
    }
}