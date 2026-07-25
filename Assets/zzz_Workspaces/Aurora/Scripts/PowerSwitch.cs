using UnityEngine;

public class PowerSwitch : MonoBehaviour
{
    public Electricity2D electricity;
    private bool switchState = true;

    public void TogglePower() {

        switchState = !switchState;
        electricity.SetActive(switchState);
    }
}