using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GuideScript : MonoBehaviour
{
    private static GuideScript Instance;
    private bool isCurrentEventFinished;
    private bool isTyping;
    private Action currentAction;

    private Queue<string> messageToSay;

    [Header("Game Objects")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject joyStick;
    [SerializeField] private GameObject settingsBtn;
    [SerializeField] private GameObject scroll;
    [SerializeField] private GameObject timeVisual;
    [SerializeField] private GameObject talk;
    [SerializeField] private TextMeshProUGUI guideText;

    [Header("Text to speech")]
    [SerializeField] private GameObject skipButton;


    [Header("Text to speech")]
    [SerializeField] private VoiceController vController;

    [Header("Arrows")]
    [SerializeField] private GameObject mmArrow;
    [SerializeField] private GameObject tmArrow;
    [SerializeField] private GameObject jsArrow;
    [SerializeField] private GameObject tkArrow;
    [SerializeField] private GameObject scArrow;

    [Header("targetPositions")]
    [SerializeField] private Transform target1;
    [SerializeField] private Transform scrollObj;
    [SerializeField] private Transform teacher;

    [Header("Confirmation")]
    [SerializeField] private MyConfirmationScript confirmation;

    public static GuideScript GetInstance()
    {
        return Instance;
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ClearAll();
        isCurrentEventFinished = false;

        // Example start event
        Welcome();
        //PlayerPrefs.SetInt("FinishedTutorial", 0);

        if(PlayerPrefs.GetInt("FinishedTutorial", 0) == 0)
        {
            skipButton.SetActive(false);
        }
    }

    private void Welcome()
    {
        Player.FindPlayer().GetComponent<GlobalArrow>().Hide();
        messageToSay.Enqueue("Welcome to Bayani Chronicles.");
        messageToSay.Enqueue("Firstly, we will teach you how to play this game.");

        isCurrentEventFinished = false;
        Activate(new List<GameObject> { background });
        SayMessage(minimapIntro);
    }

    private void minimapIntro()
    {
        ClearAll();
        messageToSay.Enqueue("This is the Mini Map");
        messageToSay.Enqueue("Where you can see the Map with a wider View");

        isCurrentEventFinished = false;
        Activate(new List<GameObject> { background,miniMap,mmArrow });
        SayMessage(TimeIntro);
    }

    private void TimeIntro()
    {
        ClearAll();
        messageToSay.Enqueue("This is the Timer");
        messageToSay.Enqueue("It shows the countdown and time limit of each Level");
        messageToSay.Enqueue("If the time runs out, you will lose the game");

        isCurrentEventFinished = false;
        Activate(new List<GameObject> { background, timeVisual, tmArrow });
        timeVisual.GetComponent<TimerScript>().StopTimer();
        SayMessage(JoyIntro);
    }

    private void JoyIntro()
    {
        timeVisual.GetComponent<TimerScript>().StartTimer();
        ClearAll();
        messageToSay.Enqueue("This is your Joystick");
        messageToSay.Enqueue("Use this to move around the map as you Please");

        isCurrentEventFinished = false;
        Activate(new List<GameObject> { background, joyStick, jsArrow });
        joyStick.GetComponent<EventTrigger>().enabled = false;
        SayMessage(TalkIntro);
    }

    private void TalkIntro()
    {
        joyStick.GetComponent<EventTrigger>().enabled = true;
        ClearAll();
        messageToSay.Enqueue("This is the Talk Button");
        messageToSay.Enqueue("Once you Get close to the Npc");
        messageToSay.Enqueue("Click this to interact with them");
        messageToSay.Enqueue("Some Npc Takes you to series of events");

        isCurrentEventFinished = false;
        Activate(new List<GameObject> { background, talk, tkArrow });
        SayMessage(FindTeacher);
    }

    private void FindTeacher()
    {
        ClearAll();

        Player.FindPlayer().GetComponent<GlobalArrow>().Show();
        Player.FindPlayer().GetComponent<GlobalArrow>().SetTarget(target1);

        messageToSay.Enqueue("You may now Go and Find you Teacher");
        messageToSay.Enqueue("Your Teacher will instruct you on what you will do");
        messageToSay.Enqueue("And they will discuss Something before you proceed to the game");
        messageToSay.Enqueue("Just follow the arrow for now to go to the teacher");
        Activate(new List<GameObject> { background });
        SayMessage(FindTeacher2);
    }

    private void FindTeacher2()
    {
        ClearAll();
        isCurrentEventFinished = false;
        Activate(new List<GameObject> { miniMap, timeVisual, joyStick });
    }
    public void TriggerPostTeacher()
    {
        Player.FindPlayer().GetComponent<GlobalArrow>().Hide();
    }

    public void GameroomIntro()
    {
        ClearAll();
        messageToSay.Enqueue("After The discussion, you are proceeded to the Gameroom Area");
        messageToSay.Enqueue("The location may varies depending on the level");
        messageToSay.Enqueue("Your goal here is to find all the scrolls");
        Activate(new List<GameObject> { background });
        isCurrentEventFinished = false;
        SayMessage(HighLightScroll);
    }

    private void HighLightScroll()
    {
        ClearAll();
        DialogueManager.Instance.GetCam().SetTargetPosition(scrollObj);
        StartCoroutine(StopAndGo(PostScroll, 5));
    }

    private void PostScroll()
    {
        ClearAll();
        messageToSay.Enqueue("Gather all the scrolls like that within this whole level");
        Activate(new List<GameObject> { background });
        isCurrentEventFinished = false;
        SayMessage(HighlightScrollCounter);
    }

    private void HighlightScrollCounter()
    {
        ClearAll();
        messageToSay.Enqueue("This is the Scroll counter");
        messageToSay.Enqueue("You need to Find all Scrolls that is written here");
        DialogueManager.Instance.GetCam().SetTargetPosition(Player.FindPlayer().transform);
        Activate(new List<GameObject> { background,scroll,scArrow });
        isCurrentEventFinished = false;
        SayMessage(GameroomTeacherIntro);
    }

    public void GameroomTeacherIntro()
    {
        ClearAll();
        messageToSay.Enqueue("Some npc are Roaming Around the area");

        Activate(new List<GameObject> { background });
        isCurrentEventFinished = false;
        SayMessage(HighlightGameroomTeacher);
    }

    private void HighlightGameroomTeacher()
    {
        ClearAll();
        DialogueManager.Instance.GetCam().SetTargetPosition(teacher);
        StartCoroutine(StopAndGo(PostSGameroomTeachercroll, 5));
    }

    private void PostSGameroomTeachercroll()
    {
        ClearAll();
        messageToSay.Enqueue("Interact with them");
        messageToSay.Enqueue("Some Npc Like this teacher ask you a question");
        messageToSay.Enqueue("Failed to answer the question may lead to deduction in time");
        messageToSay.Enqueue("And if you answer it right");
        messageToSay.Enqueue("They will show a hidden scrolls or maybe They will open a Restricted Area");
        Activate(new List<GameObject> { background });
        isCurrentEventFinished = false;
        SayMessage(ProceedToGame);
    }

    private void ProceedToGame()
    {
        ClearAll();
        Activate(new List<GameObject> { miniMap, timeVisual, scroll, joyStick });
        DialogueManager.Instance.GetCam().SetTargetPosition(Player.FindPlayer().transform);
    }

    public void Final()
    {
        ClearAll();
        messageToSay.Enqueue("Once You have Completed all the scrolls");
        messageToSay.Enqueue("You are now proceed to answer all the quiz questions");
        messageToSay.Enqueue("These questions are Final");
        messageToSay.Enqueue("Meaning, it will be recorded to your Records");
        messageToSay.Enqueue("Every failed attempt are being recorded as 0 remarks");
        messageToSay.Enqueue("That's all the turorial you need");
        messageToSay.Enqueue("Congratulations!! You may now proceed to the actual Game");
        Activate(new List<GameObject> { background });
        isCurrentEventFinished = false;
        SayMessage(Return);
    }

    public void Return()
    {
        PlayerPrefs.SetInt("FinishedTutorial", 1);
        SceneManager.LoadScene("MainScene");
    }

    IEnumerator StopAndGo(Action nextAction, float seconds = 5f)
    {
        ClearAll();
        yield return new WaitForSeconds(seconds);
        nextAction.Invoke();
    }

    private void ClearAll()
    {
        background.SetActive(false);
        miniMap.SetActive(false);
        joyStick.SetActive(false);
        settingsBtn.SetActive(false);
        scroll.SetActive(false);
        timeVisual.SetActive(false);
        talk.SetActive(false);
        mmArrow.SetActive(false);
        tmArrow.SetActive(false);
        jsArrow.SetActive(false);
        tkArrow.SetActive(false);
        scArrow.SetActive(false);

        if (guideText != null)
            guideText.text = "";

        messageToSay = new Queue<string>();
    }

    private void Activate(List<GameObject> gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    public void ClickNext()
    {
        if (isTyping)
        {
            // If player clicks while text is still typing, instantly show full line
            StopAllCoroutines();
            guideText.text = currentFullMessage;
            isTyping = false;
            isCurrentEventFinished = true;
            vController.StopSpeaking();
            return;
        }

        if (isCurrentEventFinished)
        {
            if (messageToSay.Count > 0)
            {
                // Continue to next message
                SayMessage(currentAction);
            }
            else
            {
                // No more messages, trigger final action
                currentAction?.Invoke();
            }
        }
    }

    private void Finish()
    {
        ClearAll();
        Debug.Log("Finished tutorial section.");
    }

    private string currentFullMessage = "";

    public void SayMessage(Action nextAction)
    {
        if (messageToSay.Count > 0)
        {
            string message = messageToSay.Dequeue();
            StartCoroutine(TypeSentence(message, 0.03f));
            currentAction = nextAction;
        }
        else
        {
            nextAction?.Invoke();
        }
    }

    private IEnumerator TypeSentence(string message, float speed)
    {
        if (guideText == null)
        {
            Debug.LogWarning("GuideText is not assigned in the Inspector!");
            yield break;
        }

        guideText.text = "";
        currentFullMessage = message;
        isTyping = true;
        isCurrentEventFinished = false;
        vController.StartSpeaking(message);

        foreach (char letter in message.ToCharArray())
        {
            guideText.text += letter;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
        isCurrentEventFinished = true;
    }

    public void Skip()
    {
        confirmation.OpenConfirmation("Are you sure you want to skip the Tutorial?", Return);
    }
}
