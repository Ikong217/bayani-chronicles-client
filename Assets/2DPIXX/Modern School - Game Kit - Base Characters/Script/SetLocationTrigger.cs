using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class SetLocationCharacter 
{
    public bool player = true;
    public GameObject character;
    //AnimationScript anim;
    public Transform tartgetLocation;
    public DirectionType direction = DirectionType.RIGHT;
    public bool walk;
    public bool camFollow = true;
    public float cooldown = 0f;
    //public float adjustX = 0f;
    //public float adjustY = 0f;
}
[System.Serializable]
public class SetLocation
{
    public List<SetLocationCharacter> setLocatoins = new List<SetLocationCharacter>();
}

public class SetLocationTrigger : MonoBehaviour
{
    public SetLocation setLocation;

    [Header("Interctable")]
    public bool required = false;
    private Button talkButton;
    private GameObject dialogueManager;
    public bool reusable = false;
    public ObjectsContainer affectedObjects;
    public ObjectsContainer activateObjects;

    void Start()
    {
        GameObject coreComponent = GameObject.Find("Core Level Components");
        dialogueManager = coreComponent.transform.Find("DalogueManager").gameObject;
        talkButton = dialogueManager.GetComponent<TalkScript>().GetTalkButton();
    }

    private void TriggerEvents()
    {
        StartCoroutine(WaitForFinishCoroutine());
    }

    private IEnumerator WaitForFinishCoroutine()
    {

        SetLocationManager setLocationManager = SetLocationManager.Instance;
        setLocationManager.StartSetLocations(setLocation);

        while (!setLocationManager.inTargetposition)
        {
            yield return null;
        }

        // Only after finishing, continue
        if (reusable)
        {
        }
        else
        {
            if (!required) talkButton.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        if (!required)
            talkButton.gameObject.SetActive(true);

        ObjectsHandler.DestroyObject(affectedObjects, gameObject);
        ObjectsHandler.ActivateObject(activateObjects);
    }

    public void Talk()
    {
        //TriggerDialogue();
        TriggerEvents();
        talkButton.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Player.FindPlayer().GetComponent<CharacterMovement>().onGoingEvent){
             if (collision.CompareTag("Offset"))
            {
                if (required)
                {
                    //TriggerDialogue();
                    TriggerEvents();
                }
                else
                {
                    //print("showButton");
                    talkButton.gameObject.SetActive(true);
                    talkButton.onClick.RemoveAllListeners();
                    talkButton.onClick.AddListener(Talk);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Offset"))
        {
            if (!required)
            {
                talkButton.gameObject.SetActive(false);
            }
        }
    }
}
