using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetLocationManager : MonoBehaviour
{
    public static SetLocationManager Instance;

    private Queue<SetLocationCharacter> targetLocations = new Queue<SetLocationCharacter>();
    private bool isWalking = false;
    private GameObject character;
    private Transform endPosition;
    private float cooldown = 0f;

    public bool inTargetposition = false;
    private bool subEvent = false;
    private MiscManager miscManager;
    private Camera cam;
    private CamFollow camscr;
    private float alignOffset = 0f;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        miscManager = gameObject.GetComponent<MiscManager>();
        GameObject coreComponent = GameObject.Find("Core Level Components");
        cam = coreComponent.transform.Find("Main Camera").GetComponent<Camera>();
        camscr = cam.GetComponent<CamFollow>();
    }


    public void StartSetLocations(SetLocation setLocation)
    {
        alignOffset = 0f;
        // Reset states
        inTargetposition = false;
        targetLocations.Clear();

        // Enqueue all target locations
        foreach (SetLocationCharacter location in setLocation.setLocatoins)
        {
            targetLocations.Enqueue(location);
        }
        if(targetLocations.Count < 1)
        {
            EndLocation();
            return;
        }

        if (miscManager.isRunning())
        {
            subEvent = true;
        }
        else
        {
            subEvent = false;
            miscManager.StartEvent();
        }

        StartCoroutine(WaitSectonds(1f));

        // Start moving
        StartCoroutine(HandleMovementSequence());
    }

    IEnumerator WaitSectonds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator HandleMovementSequence()
    {
        while (targetLocations.Count > 0)
        {
            SetLocationCharacter currentLocation = targetLocations.Dequeue();
            if (currentLocation.player)
            {
                character = Player.FindPlayer();
            }
            else
            {
                character = currentLocation.character;
            }

            ItemLayeringScript layering = character.GetComponent<ItemLayeringScript>();

            if (layering != null) layering.TurnOff();

            //NPCWander2D wander = character.GetComponent<NPCWander2D>(); //reject

            //if (wander != null) wander.TurnOff();

            BoxCollider2D characterCollider = character.GetComponent<BoxCollider2D>();
            if (characterCollider != null)
                characterCollider.enabled = false;

            CharAnimation charAnimation = character.gameObject.GetComponent<CharAnimation>();
            if (currentLocation.camFollow)
                camscr.SetTargetPosition(character.transform);
            else
                camscr.SetTargetPosition(Player.FindPlayer().transform);

            //if (currentLocation.walk)
            //{
            //    characterController.AddSpeed();
            //}
            //else
            //{
            //    characterController.RemoveSpeed();
            //}
            //
            //characterController.SetDirection(currentLocation.direction);

            cooldown = currentLocation.cooldown;
            //print("start");

            if (currentLocation.walk)
            {
                //print("walk");
                endPosition = currentLocation.tartgetLocation;
                if(character.GetComponent<CharAnimation>() != null && character != Player.FindPlayer() && currentLocation.tartgetLocation.gameObject == Player.FindPlayer()) {
                    alignOffset = 0.62f;
                }

                //endPosition.position = new Vector2(
                //    endPosition.position.x + currentLocation.adjustX,
                //    endPosition.position.y + currentLocation.adjustY);

                isWalking = true;
                CharAnimation charAnim = character.GetComponent<CharAnimation>();

                // Walk towards the target
                while (isWalking)
                {
                    float step = 2.5f * Time.deltaTime;
                    Vector3 endingPos = new Vector3(endPosition.position.x, endPosition.position.y + alignOffset, endPosition.position.z);
                    character.transform.position = Vector3.MoveTowards(character.transform.position, endingPos, step);
                    if (Vector3.Distance(character.transform.position, endingPos) < 0.01f)
                    {
                        isWalking = false;
                    }
                    if (charAnim)
                    {
                        charAnim.Walking(true);
                    }
                    yield return null;
                }

                if (charAnim)
                {
                    charAnim.Walking(false);
                }

                if (layering != null) layering.TurnOn();
            }
            else
            {
                //print(currentLocation.direction);
                //charAnimation.ForceSetDirection(currentLocation.direction);
                
                charAnimation.SetDirection(currentLocation.direction);
                //print(currentLocation.direction);
            }

            //if (wander != null) wander.TurnOn();

            if (characterCollider != null)
                characterCollider.enabled = true;

            yield return null; // wait for next frame

            // After reaching, wait for cooldown
            if (cooldown > 0)
            {
                yield return new WaitForSeconds(cooldown);
            }
        }

        // All locations finished
        EndLocation();
    }

    void EndLocation()
    {
        if (subEvent)
        {
            subEvent = false;
        }
        else
        {
            miscManager.EndEvent();
        }

        inTargetposition = true;
    }

    public GameObject GetCharacter() => character;
}
