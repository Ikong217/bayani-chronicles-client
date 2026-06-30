using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Events
{
    public Dialogue dialogue;
    public SetLocation setLocation;
    public SetCharacterEmotes setEmotes;
    public NPCQuestions questions;
    public EventType eventType = EventType.dialogue;
    public float interval = 0f;
}
[System.Serializable]
public enum EventType
{
    dialogue,
    setLocation,
    setEmotes,
    questions
}

[System.Serializable]
public class SetEvents
{
    public List<Events> events = new List<Events>();
}

[System.Serializable]
public class ObjectsContainer
{
    public List<GameObject> container = new List<GameObject>();
}

public static class ObjectsHandler 
{
    public static void DestroyObject(ObjectsContainer container, GameObject exception = null)
    {
        List<GameObject> items = container.container;
        if(items.Count >= 1)
        {
            foreach(GameObject item in items)
            {
                if(item != null && item != exception)
                {
                    MonoBehaviour.Destroy(item);
                }
            }
        }
    }

    public static void ActivateObject(ObjectsContainer container)
    {
        List<GameObject> items = container.container;
        if (items.Count >= 1)
        {
            foreach (GameObject item in items)
            {
                if (item != null)
                {
                    item.SetActive(true);
                }
            }
        }
    }
}

public class EventsTrigger : MonoBehaviour
{
    //public Dialogue dialogue;
    //public SetLocation setLocation;
    public SetEvents setEvents;

    [Header("Interctable")]
    public bool required = false;
    private GameObject dialogueManager;
    private Button talkButton;
    private BoxCollider2D boxCollider;
    public bool reusable = false;
    public ObjectsContainer affectedObjects;
    public ObjectsContainer activateObjects;

    //[Header("For Player Only")]
    //private BoxCollider2D playerCollider;
    //private GameObject offset;
    //private BoxCollider2D offsetCollider;
    void Start()
    {
        //print(System.DateTime.Now.TimeOfDay);
        //Transform offsetTransform = player.transform.Find("Offset(Clone)");
        //if (offsetTransform == null)
        //{
        //    Debug.LogError("Offset child not found under Player");
        //    return;
        //}
        //offset = offsetTransform.gameObject;
        boxCollider = GetComponent<BoxCollider2D>();
        //offsetCollider = offset.GetComponent<BoxCollider2D>();
        //playerCollider = player.GetComponent<BoxCollider2D>();
        GameObject coreComponent = GameObject.Find("Core Level Components");
        dialogueManager = coreComponent.transform.Find("DalogueManager").gameObject;
        //print(dialogueManager);
        talkButton = dialogueManager.GetComponent<TalkScript>().GetTalkButton();
    }


    /* public void TriggerDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager.Instance is null at TriggerDialogue");
            return;
        }

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void TriggerLocation()
    {
        if (SetLocationManager.Instance == null)
        {
            Debug.LogError("SetLocationManager.Instance is null at TriggerDialogue");
            return;
        }
        //print("happy");
        SetLocationManager.Instance.StartSetLocations(setLocation);
    } */

    private void TriggerEvents()
    {
        //playerCollider.enabled = false;
        //offsetCollider.enabled = false;
        //boxCollider.enabled = false;
        StartCoroutine(WaitForFinishCoroutine());
    }

    IEnumerator WaitForFinishCoroutine()
    {
        EventsManager eventManager = EventsManager.Instance;
        eventManager.StartEvent(setEvents);

        // Wait until all events are finished
        while (!eventManager.eventFinish)
        {
            yield return null;
        }

        ObjectsHandler.DestroyObject(affectedObjects,gameObject);
        ObjectsHandler.ActivateObject(activateObjects);

        // Only after finishing, continue
        if (reusable)
        {
            //boxCollider.enabled = true;
        }
        else
        {
            if (!required) talkButton.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }

        if (!required)
            talkButton.gameObject.SetActive(false);
        //playerCollider.enabled = true;
        //offsetCollider.enabled = true;
    }


    public void Talk()
    {
        print("talk");
        //TriggerDialogue();
        TriggerEvents();
        talkButton.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!Player.FindPlayer().GetComponent<CharacterMovement>().onGoingEvent)
        {

            if (collision.CompareTag("Offset"))
            {
                if (required)
                {
                    //TriggerDialogue();
                    TriggerEvents();
                }
                else
                {
                    print("showButton");
                    talkButton.gameObject.SetActive(true);
                    talkButton.onClick.RemoveAllListeners();
                    talkButton.onClick.AddListener(Talk);
                }
                //TriggerLocation();
                //Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Offset") && !required)
        {
            talkButton.gameObject.SetActive(false);
        } 
    }
}

public static class Player
{
    public static GameObject FindPlayer()
    {

        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.Find("PlayerMale");
        }
        if (player == null)
        {
            player = GameObject.Find("PlayerFemale");
        }
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player == null)
        {
            Debug.LogError("Player not found with name 'Player'");  
        }

        return player;
    }
}