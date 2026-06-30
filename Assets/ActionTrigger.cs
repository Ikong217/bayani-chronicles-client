using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ActionTrigger : MonoBehaviour
{

    public ObjectsContainer affectedObjects;
    public ObjectsContainer activateObjects;
    private GameObject dialogueManager;
    private Button talkButton;
    public bool reusable = false;
    public bool required = false;

    public Action whatToDo;
    public Do _do;

    private void Start()
    {
        GameObject coreComponent = GameObject.Find("Core Level Components");
        dialogueManager = coreComponent.transform.Find("DalogueManager").gameObject;
        talkButton = dialogueManager.GetComponent<TalkScript>().GetTalkButton();

        switch (_do)
        {
            case Do.postTeacher:
                whatToDo = () => GuideScript.GetInstance().TriggerPostTeacher();
                break;
            case Do.gameroomIntro:
                whatToDo = () => GuideScript.GetInstance().GameroomIntro();
                break;
            case Do.finish:
                print("process finish");
                whatToDo = () => GuideScript.GetInstance().Final();
                break;

            default:
                print("Do Nothing");
                break;
        }
    }


    private void TriggerEvents()
    {
        whatToDo.Invoke();
        ObjectsHandler.DestroyObject(affectedObjects, gameObject);
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

    public enum Do
    {
        postTeacher,
        gameroomIntro,
        finish
    }
}
