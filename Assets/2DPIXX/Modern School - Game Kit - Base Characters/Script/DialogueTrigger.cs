using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public enum DialogueType
{
    character,
    item
}
//[System.Serializable]
//public class DialogueCharacter
//{
//    public string name;
//    public Sprite icon;
//}
[System.Serializable]
public class DialogueLine
{
    //public DialogueCharacter character;
    public GameObject characterObj;
    public bool player = true;
    public bool cameraFollow = true;
    public Sprite sprite;
    public DialogueType dialogueType = DialogueType.character;
    [TextArea(3, 10)]
    public string line = "";
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}


public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    [Header("Interctable")]
    public bool required = false;
    private Button talkButton;
    private GameObject dialogeManager;
    public bool reusable = true;
    public ObjectsContainer affectedObjects;
    public ObjectsContainer activateObjects;
    [Header("Exclusive")]
    public bool Randomize = false;

    private GameObject player;
    void Start()
    {
        player = Player.FindPlayer();

        GameObject coreComponent = GameObject.Find("Core Level Components");
        dialogeManager = coreComponent.transform.Find("DalogueManager").gameObject;
        talkButton = dialogeManager.GetComponent<TalkScript>().GetTalkButton();
    }

    private void TriggerEvents()
    {
        StartCoroutine(WaitForFinishCoroutine());
    }

    public void PubTrigger()
    {
        TriggerEvents();
    }

    private IEnumerator WaitForFinishCoroutine()
    {
        DialogueManager dialogueManager = DialogueManager.Instance;
        Dialogue toSay = dialogue;

        if (Randomize && dialogue.dialogueLines.Count > 0)
        {
            toSay = new Dialogue();
            int randIndex = Random.Range(0, dialogue.dialogueLines.Count);
            DialogueLine randomLine = dialogue.dialogueLines[randIndex];
            toSay.dialogueLines.Add(randomLine);
        }

        dialogueManager.StartDialogue(toSay);

        while (!dialogueManager.finished)
        {
            yield return null;
        }

        // Only after finishing, continue
        if (!reusable)
            Destroy(this.gameObject);

        if (!required && !reusable) talkButton.gameObject.SetActive(false);

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
        if (!isActiveAndEnabled)
            return;

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
                    //print("showButton");
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
