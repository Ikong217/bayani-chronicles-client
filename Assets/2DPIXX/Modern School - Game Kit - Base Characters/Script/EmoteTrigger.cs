using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class EmoteEventVars
{
    public bool player;
    public GameObject character;
    public Emotes emotes = Emotes.none;
    public float duration = 0f;
}

[System.Serializable]
public class SetCharacterEmotes
{
    public List<EmoteEventVars> emotions = new List<EmoteEventVars>();
}

public class EmoteTrigger : MonoBehaviour
{
    public SetCharacterEmotes setCharacterEmotes;
    public float interval = 0.1f;

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

        EmoteManager emoteManager = EmoteManager.Instance;
        emoteManager.StartEmote(setCharacterEmotes);

        while (!emoteManager.emoteFinished)
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

        yield return new WaitForSeconds(interval);

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
        if (!Player.FindPlayer().GetComponent<CharacterMovement>().onGoingEvent)
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
