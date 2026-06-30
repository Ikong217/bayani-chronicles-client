using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance;
    public Queue<Events> events = new Queue<Events>();
    private bool onGoingEvent = false;
    public bool eventFinish = false;
    private MiscManager miscManager;
    private void Awake()
    {
        miscManager = gameObject.GetComponent<MiscManager>();
    }
    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartEvent(SetEvents setEvents)
    {
        miscManager.StartEvent();
        foreach (Events ev in setEvents.events)
        {
            events.Enqueue(ev);
        }

        onGoingEvent = true;
        eventFinish = false;
        StartNextEvent();
    }

    public void StartNextEvent()
    {
        if (events.Count == 0)
        {
            EndEvent();
            return;
        }

        Events ev = events.Dequeue();
        StartCoroutine(ApplyEvent(ev));
    }

    private IEnumerator ApplyEvent(Events ev)
    {
        if(ev.eventType == EventType.dialogue)
        {
            Dialogue dialogue = ev.dialogue;
            DialogueManager dialogueManager = DialogueManager.Instance;
            dialogueManager.StartDialogue(dialogue);
            
            // Wait for the dialogue to finish before continuing
            yield return StartCoroutine(WaitDialogueFinish(dialogueManager));

        }

        if (ev.eventType == EventType.setLocation)
        {
            SetLocation setLocation = ev.setLocation;
            SetLocationManager setLocationManager = SetLocationManager.Instance;
            setLocationManager.StartSetLocations(setLocation);

            // Wait for the location to finish after the dialogue finishes
            yield return StartCoroutine(WaitLocationFinish(setLocationManager));

        }

        if (ev.eventType == EventType.setEmotes)
        {
            SetCharacterEmotes setCharEmotes= ev.setEmotes;
            EmoteManager emoteManager = EmoteManager.Instance;
            emoteManager.StartEmote(setCharEmotes);

            // Wait for the dialogue to finish before continuing
            yield return StartCoroutine(WaitEmoteFinished(emoteManager));

        }

        if (ev.eventType == EventType.questions)
        {
            NPCQuestions questions = ev.questions;
            QuestionManager qManager= QuestionManager.Instance;
            qManager.StartQuestion(questions,questions.cycle);

            // Wait for the dialogue to finish before continuing
            yield return StartCoroutine(WaitQuestionFinish(qManager));

        }

        // Wait for the interval before moving to the next event
        yield return new WaitForSeconds(ev.interval);

        // Proceed to the next event
        StartNextEvent();
    }

    private IEnumerator WaitQuestionFinish(QuestionManager questionManager)
    {
        while (!questionManager.questionFinished)
        {
            yield return null;
        }
    }

    private IEnumerator WaitEmoteFinished(EmoteManager emoteManager)
    {
        while (!emoteManager.emoteFinished)
        {
            yield return null;
        }
    }

    private IEnumerator WaitDialogueFinish(DialogueManager dialogueManager)
    {
        while (!dialogueManager.finished)
        {
            //print("not yet finished");
            yield return null;
        }
    }

    private IEnumerator WaitLocationFinish(SetLocationManager setLocationManager)
    {
        // Check the location manager's inTargetposition flag, not the dialogue manager's flag
        while (!setLocationManager.inTargetposition)
        {
            //print("not yet finished");
            yield return null;
        }
    }

    public void EndEvent()
    {
        miscManager.EndEvent();

        onGoingEvent = false;
        eventFinish = true;
        Debug.Log("All Events Completed!");
    }
}
