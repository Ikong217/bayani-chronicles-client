using UnityEngine;
using System.Collections;

// Main class to handle the questions
public class QuestionHandler : MonoBehaviour
{
    private QuestionsContainer questionnaires;
    private QuestionnaireItems items;
    private bool questionsLoaded = false;
    public SetEvents setEvent;
    public GameObject levelComplete;
    public Coroutine completeCoroutine;

    [Header("Alotted Time For Quiz")]
    [Min(0f)] public float minute = 0f;
    [Min(0f)] public float seconds = 0f;
    
    [Header("Warning Time For Quiz")]
    [Min(0f)] public float warnMinute = 0f;
    [Min(0f)] public float warnSecond = 0f;

    [SerializeField] private bool isPerItem = false;

    private void OnEnable()
    {
        //TimerSet();
        StartCoroutine(TogglePreEvent());
    }

    private void TimerSet()
    {
        TimerScript timeScript = TimerScript.Instance;
        if(minute > 0 || seconds > 0) timeScript.SetTimer(minute, seconds, RuntimeOut, warnMinute, warnSecond);
    }

    IEnumerator TogglePreEvent()
    {
        if (TimerScript.Instance.IsTimerRunning()) TimerScript.Instance.ToggleTimer();
        EventsManager eventManager = EventsManager.Instance;
        if(setEvent != null)
            eventManager.StartEvent(setEvent);

        // Wait until all events are finished
        while (!eventManager.eventFinish)
        {
            yield return null;
        }
        StartLoadQuestions();
    }

    private void StartLoadQuestions()
    {
        TimerSet();
        if (!TimerScript.Instance.IsTimerRunning()) TimerScript.Instance.ToggleTimer();
        // Subscribe to events
        QuestionsRequestHandler.OnQuestionsLoaded += OnQuestionsLoaded;
        QuestionsRequestHandler.OnQuestionsLoadFailed += OnQuestionsLoadFailed;

        items = GetComponent<QuestionnaireItems>();

        // Check if questions are already loaded
        QuestionsRequestHandler request = QuestionsRequestHandler.Instance;
        if (request != null && request.questions != null && request.questions.questions.Count > 0)
        {
            PlayerPrefs.SetInt("LevelQuestionCount", request.questions.questions.Count);
            questionnaires = request.questions;
            StartQuestionnaire();
        }
        else if (!questionsLoaded)
        {
            // Questions not loaded yet, show loading message or wait
            Debug.Log("Waiting for questions to load...");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        QuestionsRequestHandler.OnQuestionsLoaded -= OnQuestionsLoaded;
        QuestionsRequestHandler.OnQuestionsLoadFailed -= OnQuestionsLoadFailed;
    }

    private void OnQuestionsLoaded(QuestionsContainer questions)
    {
        questionnaires = questions;
        questionsLoaded = true;
        StartQuestionnaire();
    }

    private void OnQuestionsLoadFailed(string error)
    {
        Debug.LogError("Failed to load questions: " + error);
        // Handle error - show message to user, etc.
    }

    private void StartQuestionnaire()
    {
        if (questionnaires != null && items != null)
        {
            PlayerPrefs.DeleteKey("PlayerScore");
            StartCoroutine(RunQuestionnaire());
        }
    }

    IEnumerator RunQuestionnaire()
    {
        foreach (QuestionsStorage questions in questionnaires.questions)
        {
            print(questions.rationalization);
        }
        foreach (QuestionsStorage questions in questionnaires.questions)
        {
            int questionIndex = questionnaires.questions.IndexOf(questions); // get index of current question
            int total = questionnaires.questions.Count; // total questions

            items.StartQuestion(questions, questionIndex,total,()=>{ 
                PlayerPrefs.SetInt("PlayerScore", PlayerPrefs.GetInt("PlayerScore", 0) + 1);
                ResetTime();
            }, ()=> { ResetTime(); });
            yield return StartCoroutine(WaitForQuestionToFinish());
        }

        // All questions finished
        Debug.Log("Questionnaire completed! Final score: " + PlayerPrefs.GetInt("PlayerScore", 0));
        ActivateComplete();
    }

    private void ResetTime()
    {
        if (isPerItem)
        {
            TimerSet();
        }
    }

    private void RuntimeOut()
    {
        items.RunTimeOut();
    }

    IEnumerator WaitForQuestionToFinish()
    {
        yield return new WaitUntil(() => items.isFinished);
    }

    private void ActivateComplete()
    {
        if(completeCoroutine != null)
        {
            print("Coroutine already started");
        }

        Time.timeScale = 0f;
        ScoreRequestHandler scoreRequest = ScoreRequestHandler.Instance;

        if(scoreRequest == null)
        {
            Debug.LogError("ScoreRequestHandler instance not found!");
            ActivateSuccessPanel();
            return;
        }
        scoreRequest.StartRequestingScoreUpdate(GameLevelPlayedStatus.Completed, PlayerPrefs.GetInt("PlayerScore", 0));
        completeCoroutine = StartCoroutine(WaitSuccessRequest(scoreRequest));
    }

    IEnumerator WaitSuccessRequest(ScoreRequestHandler requestHandler)
    {
        if (requestHandler == null)
        {
            ActivateSuccessPanel();
            yield break;
        }

        yield return new WaitUntil(() => requestHandler.externalRequest == false);
        yield return new WaitForSecondsRealtime(0.1f);

        if (requestHandler.externalSuccess)
        {
            ActivateSuccessPanel();
        }
        else
        {
            Debug.LogWarning("Score update failed, retrying restart...");
            yield return new WaitForSecondsRealtime(1f);
            ActivateComplete();
        }

        completeCoroutine = null;
    }
    private void ActivateSuccessPanel()
    {
        levelComplete.SetActive(true);
    }
}