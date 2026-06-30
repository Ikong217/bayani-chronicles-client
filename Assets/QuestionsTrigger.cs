using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class NPCQuestions
{
    public bool fromDatabase = false;
    public QuestionsContainer questions;
    public bool cycle = true;
    public bool isOneTime = false;
    public float DeductTime = 0;
}

public class QuestionsTrigger : MonoBehaviour
{
    public NPCQuestions questions;
    [Header("Interctable")]
    public bool required = false;
    private Button talkButton;
    private GameObject dialogeManager;
    public bool reusable = false;
    public ObjectsContainer affectedObjects;
    public ObjectsContainer activateObjects;

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

    private IEnumerator WaitForFinishCoroutine()
    {

        QuestionManager qManager = QuestionManager.Instance;
        qManager.StartQuestion(questions,questions.cycle);

        while (!qManager.questionFinished)
        {
            yield return null;
        }

        if (!required && !reusable) talkButton.gameObject.SetActive(false);

        if (!required)
            talkButton.gameObject.SetActive(true);

        if (qManager.IsCorrect())
        {
            // Only after finishing, continue
            if (!reusable)
                Destroy(this.gameObject);

            ObjectsHandler.DestroyObject(affectedObjects, gameObject);
            ObjectsHandler.ActivateObject(activateObjects);
        }
    }

    public void Talk()
    {
        print("talked");
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
