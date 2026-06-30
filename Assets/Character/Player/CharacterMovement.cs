using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private JoystickScript movementJoystick;
    private GameObject dialogueManager;
    public float playerSpeed = 5f;
    public float offsetHeight = 0f;

    private GameObject offSet;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private CharAnimation charAnim;
    private bool isMoving = false;
    private DirectionType direction;
    private DirectionType forcedDirection;
    private bool fDUsed = true;
    public bool onGoingEvent = false;

    void Start()
    {
        //print(movementJoystick);
        dialogueManager = GameObject.Find("DalogueManager");
        movementJoystick = dialogueManager.GetComponent<MiscManager>().joystick.GetComponent<JoystickScript>();
        GameObject offSetPrefab = Resources.Load<GameObject>("Offset");
        offSet = Instantiate(offSetPrefab, transform.position, transform.rotation, transform);

        if (!movementJoystick)
        {
            Debug.LogError("Missing Component: Movement Joystick");
        }

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        charAnim = GetComponent<CharAnimation>();

        if (charAnim)
        {
            direction = charAnim.GetInitialDirection();
        }

    }

    void Update()
    {
        Vector2 inputVector = movementJoystick.joystickVector;
        isMoving = inputVector != Vector2.zero && movementJoystick.forceSpeed != 0;

        if (isMoving)
        {
            direction = DirectionHelper.GetDirection(inputVector);
        }

        RelocateOffset(inputVector);
    }

    void FixedUpdate()
    {
        Vector2 inputVector = movementJoystick.joystickVector;

        if (inputVector != Vector2.zero)
        {
            rb.linearVelocity = inputVector.normalized * playerSpeed * movementJoystick.forceSpeed;
            charAnim.ManualAnimate(true);

            charAnim.footstepSource.pitch = GetForceSpeed();
            charAnim.SetAnimSpeed(GetForceSpeed());
        }
        else if (rb.linearVelocity != Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            charAnim.ManualAnimate(false);
            charAnim.SetAnimSpeed(1f);
        }
        else if (inputVector == Vector2.zero)
        {
            charAnim.Walking(false);
            charAnim.SetAnimSpeed(1f);
        }

        if (!fDUsed)
        {
            direction = forcedDirection;
            fDUsed = true;
            print("Lala");
        }
        Animate();
        //print(GetForceSpeed());
    }

    //public void SetDirection(DirectionType newDirection)
    //{
    //    forcedDirection = newDirection;
    //    fDUsed = false;
    //}

    public float GetForceSpeed()
    {
        return movementJoystick.forceSpeed;
    }

    private void Animate()
    {
        //print(direction);
        if (isMoving)
        {
            if (charAnim)
            {
                charAnim.Walking(isMoving);
                charAnim.SetDirection(direction);
                //print(direction);
            }
        }
        
    }

    private void RelocateOffset(Vector2 inputVector)
    {
        if (inputVector == Vector2.zero) return;

        switch (DirectionHelper.GetDirection(inputVector))
        {
            case DirectionType.FRONT:
                offSet.transform.localPosition = new Vector2(0f, -0.8f + offsetHeight);
                break;
            case DirectionType.BACK:
                offSet.transform.localPosition = new Vector2(0f, -0.2f + offsetHeight);
                break;
            case DirectionType.RIGHT:
                offSet.transform.localPosition = new Vector2(0.50f, -0.5f + offsetHeight);
                break;
            case DirectionType.LEFT:
                offSet.transform.localPosition = new Vector2(-0.50f, -0.5f + offsetHeight);
                break;
        }
    }
}
