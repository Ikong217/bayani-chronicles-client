using UnityEngine;

public class MiscManager : MonoBehaviour
{
    public GameObject joystick;
    private bool ongoingEvent = false;
    private Camera cam;
    private CamFollow camscr;

    private void Start()
    {

        GameObject coreComponent = GameObject.Find("Core Level Components");
        cam = coreComponent.transform.Find("Main Camera").GetComponent<Camera>();
        camscr = cam.GetComponent<CamFollow>();
    }

    public void StartEvent()
    {
        Player.FindPlayer().GetComponent<CharacterMovement>().onGoingEvent = true;
        ongoingEvent = true;
        JoystickScript joystickScript = joystick.GetComponent<JoystickScript>();
        joystickScript.PointerUp();
        joystick.SetActive(false);
    }

    public void EndEvent()  
    {
        Player.FindPlayer().GetComponent<CharacterMovement>().onGoingEvent = false;
        camscr.SetTargetPosition(Player.FindPlayer().transform);
        joystick.SetActive(true);
        ongoingEvent = false;
    }

    public bool isRunning()
    {
        return ongoingEvent;
    }
}
