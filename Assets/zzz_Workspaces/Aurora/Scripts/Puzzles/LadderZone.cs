using UnityEngine;

public class LadderZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        //ToggleClimb(other, true);
    }
    private void OnTriggerExit2D(Collider2D other) {
        //ToggleClimb(other, false);
    }
    private void ToggleClimb(Collider2D character, bool state)
    {
        //if (character.TryGetComponent<FirefighterController>(out var firefighter))
        //{
        //    firefighter.SetClimbingState(state);
        //}
    }
}