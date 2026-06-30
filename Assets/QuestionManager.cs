using UnityEngine;
using System.Collections;
using System;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    public bool questionFinished = true;
    private QuestionsStorage question;
    private MiscManager miscManager;
    private QuestionnaireItems qitems;
    private bool subEvent = false;
    private bool cycle;
    private Action randomize;
    private bool isOneTime;
    private bool correct;
    private float deductTime;

    private void Awake()
    {
        // ✅ Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        qitems = GetComponent<QuestionnaireItems>();
        miscManager = GetComponent<MiscManager>();

        if (qitems == null)
            Debug.LogError("[QuestionManager] Missing QuestionnaireItems component!");
        if (miscManager == null)
            Debug.LogError("[QuestionManager] Missing MiscManager component!");
    }

    public void StartQuestion(NPCQuestions questions, bool cycle = false)
    {
        if (questions == null)
        {
            Debug.LogWarning("[QuestionManager] NPCQuestions is null!");
            return;
        }

        questionFinished = false;
        this.cycle = cycle;
        isOneTime = questions.isOneTime;
        deductTime = questions.DeductTime <= 5 ? questions.DeductTime : 0;
        correct = false;

        QuestionsContainer qcont = questions.fromDatabase
            ? QuestionsRequestHandler.Instance?.questions
            : questions.questions;

        if (qcont == null || qcont.questions == null || qcont.questions.Count <= 0)
        {
            Debug.LogWarning("[QuestionManager] No questions available!");
            EndQuestion();
            return;
        }

        randomize = () => RandomizeQuestion(qcont);

        // ✅ Randomize the first question immediately
        randomize.Invoke();

        // ✅ Decide whether to start a new event or stay in sub-event mode
        if (miscManager != null && miscManager.isRunning())
        {
            subEvent = true;
        }
        else
        {
            subEvent = false;
            miscManager?.StartEvent();
        }

        HandleQuestionAction();
    }

    private void RandomizeQuestion(QuestionsContainer qcont)
    {
        if (qcont == null || qcont.questions == null || qcont.questions.Count == 0)
        {
            Debug.LogWarning("[QuestionManager] RandomizeQuestion: No available questions!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, qcont.questions.Count);
        question = qcont.questions[randomIndex];
    }

    private void HandleQuestionAction()
    {
        if (qitems == null)
        {
            Debug.LogError("[QuestionManager] QuestionnaireItems not found!");
            EndQuestion();
            return;
        }

        if (question == null)
        {
            Debug.LogWarning("[QuestionManager] No question selected, ending event.");
            EndQuestion();
            return;
        }

        // ✅ Match exact parameter signature of QuestionnaireItems.StartQuestion
        qitems.StartQuestion(
            question,
            null, // index (optional)
            null, // total (optional)
            () => {
                correct = true;
                EndQuestion();
                }, // correctAction
            () => HandleIncorrectApproach() // incorrectAction
        );
    }

    private void HandleIncorrectApproach()
    {
        Debug.Log("[QuestionManager] Wrong answer, deducting time...");
        if (deductTime <= 5)
        {
            TimerScript.Instance?.DeductTimer(20f);
        }

        if (!isOneTime)
        {
            StartCoroutine(HandleNextQuestionAfterDelay(1f, cycle));
        }
        else
        {
            EndQuestion();
        }
    }

    private IEnumerator HandleNextQuestionAfterDelay(float delay, bool cycle)
    {
        yield return new WaitForSeconds(delay);

        if(cycle)randomize?.Invoke();
        HandleQuestionAction();
    }

    public void EndQuestion()
    {
        if (!subEvent)
            miscManager?.EndEvent();

        subEvent = false;
        questionFinished = true;

        //Debug.Log("[QuestionManager] Question event ended.");
    }

    public bool IsCorrect() => correct;
}
