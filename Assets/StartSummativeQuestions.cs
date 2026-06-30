using UnityEngine;
using System.Collections;

public class StartSummativeQuestions : MonoBehaviour
{
    private SummativeQuestionItems qItems;
    private SummativeRequestHandler summativeRequestHandler;
    private SummativeDataLog data;

    [SerializeField] GameObject Tally;

    private void Start()
    {
        Tally.SetActive(false);

        summativeRequestHandler = SummativeRequestHandler.Instance;
        qItems = GetComponent<SummativeQuestionItems>();

        if (qItems == null)
        {
            Debug.LogError("SummativeQuestionItems component not found!");
            return;
        }

        TimerScript.Instance.StopTimer();

        data = SummativeDataLog.Load();
        if (data == null)
            data = new SummativeDataLog();

        print(data.ToJson());

        StartCoroutine(WaitFetchData());
    }

    IEnumerator WaitFetchData()
    {
        Debug.Log("Waiting for questions...");

        SummativeQuestionData sumQD;

        // 🔁 RESUME unfinished attempt
        if (data.logs.Count > 0 && string.IsNullOrEmpty(data.logs[^1].finishedTime))
        {
            print("ols");
            sumQD = data.logs[^1];

            System.DateTime end = System.DateTime.Parse(sumQD.endTime);
            System.TimeSpan remaining = end - System.DateTime.Now;

            //yield return new WaitForSeconds(0.5f);

            if (remaining.TotalSeconds <= 0)
            {
                Finish(sumQD);
                yield break;
            }

            TimerScript.Instance.SetTimer(
                (float)remaining.Minutes,
                (float)remaining.Seconds,
                () =>
                {
                    StopAllCoroutines();
                    Finish(sumQD);
                }
            );
        }
        else
        {
            print("new");
            summativeRequestHandler.Req();
            // 🆕 NEW attempt
            yield return new WaitUntil(() => summativeRequestHandler.DataGathered());

            sumQD = new SummativeQuestionData(
                summativeRequestHandler.novel,
                summativeRequestHandler.questions
            );

            data.logs.Add(sumQD);

            TimerScript.Instance.SetTimer(
                60f,
                0f,
                () =>
                {
                    StopAllCoroutines();
                    Finish(sumQD);
                }
            );
        }

        print("Timer Started");

        TimerScript.Instance.StartTimer();

        foreach (var question in sumQD.summativeQuestionContainer.questions)
        {
            if (!string.IsNullOrEmpty(question.choice))
                continue;

            int index = sumQD.summativeQuestionContainer.questions.IndexOf(question);

            qItems.StartQuestion(
                question,
                index,
                sumQD.summativeQuestionContainer.questions.Count,
                HandleResponse
            );

            yield return new WaitUntil(() => qItems.isFinished);
        }

        Finish(sumQD);
    }

    public void Finish(SummativeQuestionData sumQD)
    {
        if (!string.IsNullOrEmpty(sumQD.finishedTime))
            return; // prevent double finish

        sumQD.Finish();
        data.Save();

        TimerScript.Instance.StopTimer();
        Tally.SetActive(true);

        Debug.Log($"All questions completed! data:{data.ToJson()}");
        ProgressData.Altered();
    }

    private void HandleResponse(
        SummativeQuestionsStorage question,
        string correctAnswer,
        string choice,
        bool isCorrect)
    {
        question.correctAnswer = correctAnswer;
        question.choice = choice;
        question.isCorrect = isCorrect;
        data.Save();
    }
}
