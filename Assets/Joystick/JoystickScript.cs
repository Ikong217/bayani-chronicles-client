using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickScript : MonoBehaviour
{
    public GameObject joystickPanel;
    public GameObject joystick;
    public GameObject joystickBackground;
    public Vector2 joystickVector;
    public float forceSpeed;
    public bool isFixed = false;

    private Vector2 touchPos;
    private Vector2 joystickOriginalPos;
    private float joystickRadius;
    public bool onTouch = false;
    //private Vector2 JSBackLastPos;

    void Start()
    {
        joystickOriginalPos = joystickBackground.transform.position;
        joystickRadius = joystickBackground.GetComponent<RectTransform>().sizeDelta.y;
        forceSpeed = 0f;
    }

    public void PointerDown()
    {
        isFixed = (PlayerPrefs.GetInt("playerJsActive", 0) == 1);
        //JSBackLastPos = joystickOriginalPos;

        if (!isFixed)
        {
            Vector2 mousePos = Input.mousePosition;
            joystickBackground.transform.position = mousePos;
            joystick.transform.position = mousePos;
            touchPos = mousePos;
        }
        else
        {
            touchPos = joystickBackground.transform.position;
        }

        onTouch = true;
    }

    public void OnHold(BaseEventData baseEventData)
    {
        Drag(baseEventData);
    }

    public void Drag(BaseEventData baseEventData)
    {
        if (!onTouch) return;

        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;

        // Calculate direction vector
        Vector2 dir = dragPos - touchPos;
        float distance = dir.magnitude;

        joystickVector = dir.normalized;

        // Clamp distance
        float clampedDist = Mathf.Min(distance, joystickRadius);
        joystick.transform.position = touchPos + joystickVector * clampedDist;

        forceSpeed = clampedDist / joystickRadius;

        // When unfixed, move background if finger goes too far
        if (!isFixed && distance > joystickRadius)
        {
            Vector2 overflow = joystickVector * (distance - joystickRadius);
            joystickBackground.transform.position += (Vector3)overflow;
            touchPos += overflow;
        }
    }

    public void PointerUp()
    {
        onTouch = false;
        joystickVector = Vector2.zero;
        forceSpeed = 0f;

        if (!isFixed)
        {
            // Move back to original position if dynamic
            joystickBackground.transform.position = joystickOriginalPos;
        }

        joystick.transform.position = joystickBackground.transform.position;
    }

    public bool activeSelf()
    {
        return gameObject.activeSelf;
    }
}
