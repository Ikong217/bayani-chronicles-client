using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;

public class SummativeRequestHandler : MonoBehaviour
{
    public static SummativeRequestHandler Instance;

    public Novels novel;
    public SummativeQuestionContainer questions;

    // Events
    public static event Action<SummativeQuestionContainer> OnQuestionsLoaded;
    public static event Action<string> OnQuestionsLoadFailed;

    private const int MAX_RETRY = 3;
    private int retryCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        //RequestQuestions(novel);
    }

    public void Req()
    {
        RequestQuestions(novel);
    }

    public void RequestQuestions(Novels novel)
    {
        this.novel = novel;
        retryCount = 0;
        StartCoroutine(HandleQuestionRequestData(novel));
    }

    IEnumerator HandleQuestionRequestData(Novels novel)
    {
        string uri = LaravelRequest.GetLink("/questions/summative/request");

        // ✅ VALID JSON BODY
        var payload = new
        {
            novel = EnumHelper.GetNovel(novel)
        };

        string jsonPayload = JsonConvert.SerializeObject(payload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                HandleQuestionsErrorResponse(www);
            }
            else
            {
                HandleQuestionsSuccessResponse(www);
            }
        }
    }

    void HandleQuestionsErrorResponse(UnityWebRequest www)
    {
        retryCount++;

        if (retryCount >= MAX_RETRY)
        {
            OnQuestionsLoadFailed?.Invoke("Failed to load questions after multiple attempts.");
            return;
        }

        try
        {
            string jsonText = www.downloadHandler.text;
            logResponseData response = JsonConvert.DeserializeObject<logResponseData>(jsonText);

            if (response != null && response.status == "error")
            {
                string errorMessage = response.message;

                if (response.errors != null)
                {
                    foreach (var error in response.errors)
                    {
                        foreach (string msg in error.Value)
                        {
                            errorMessage += "\n- " + msg;
                        }
                    }
                }

                OnQuestionsLoadFailed?.Invoke(errorMessage);
            }
            else
            {
                StartCoroutine(HandleQuestionRequestData(novel));
            }
        }
        catch (Exception e)
        {
            OnQuestionsLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    void HandleQuestionsSuccessResponse(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            //print(jsonText);

            SummativeQuestionResponse response =
                JsonConvert.DeserializeObject<SummativeQuestionResponse>(jsonText);

            if (response == null || response.status != "success" || response.data == null)
            {
                OnQuestionsLoadFailed?.Invoke("Invalid response format");
                return;
            }

            questions = new SummativeQuestionContainer
            {
                questions = response.data.questions
            };

            print(questions.questions.Count);

            OnQuestionsLoaded?.Invoke(questions);
        }
        catch (Exception e)
        {
            OnQuestionsLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    public bool DataGathered()
    {
        return questions.questions.Count > 0;
    }
}

[System.Serializable]
public class SummativeQuestionsStorage
{
    public int id;
    public string type;
    public string question;
    public string answer;
    public string[] otherAnswers;
    public string rationalization;
    public string correctAnswer;
    public string choice;
    public bool isCorrect;

    public SummativeQuestionsStorage(
        int id,
        string type,
        string question,
        string answer,
        string[] otherAnswers,
        string rationalization,
        string correctAnswer = null,
        string choice = null,
        bool isCorrect = false
    )
    {
        this.id = id;
        this.type = type;
        this.question = question;
        this.answer = answer;
        this.otherAnswers = otherAnswers;
        this.rationalization = rationalization;
        this.correctAnswer = correctAnswer;
        this.choice = choice;
        this.isCorrect = isCorrect;
    }
}


[System.Serializable]
public class SummativeQuestionResponse
{
    public string status;
    public SummativeQuestionContainer data;
}

[System.Serializable]
public class SummativeQuestionContainer
{
    public List<SummativeQuestionsStorage> questions = new List<SummativeQuestionsStorage>();
}


[System.Serializable]
public class SummativeQuestionData
{
    public Novels novel;
    public SummativeQuestionContainer summativeQuestionContainer;
    public string startTime;
    public string endTime;
    public string finishedTime;
    public int score;

    public SummativeQuestionData(
        Novels novel,
        SummativeQuestionContainer summativeQuestionContainer
    )
    {
        this.novel = novel;
        this.summativeQuestionContainer = summativeQuestionContainer;

        startTime = DateTime.Now.ToString();
        endTime = DateTime.Now.AddHours(1).ToString();
        finishedTime = ""; // set when actually finished
        score = 0;
    }

    public void Finish()
    {
        finishedTime = DateTime.Now.ToString();
        score = GetScore();
    }

    public int GetScore()
    {
        int returnScore = 0;

        foreach (SummativeQuestionsStorage data in summativeQuestionContainer.questions)
        {
            if (data.isCorrect)
                returnScore++;
        }

        return returnScore;
    }
}

[System.Serializable]
public class SummativeDataLog
{
    public List<SummativeQuestionData> logs = new List<SummativeQuestionData>();

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static SummativeDataLog FromJson(string json)
    {
        return JsonUtility.FromJson<SummativeDataLog>(json);
    }

    public void Save()
    {
        PlayerPrefs.SetString("SummativeDataLogs", ToJson());
        PlayerPrefs.Save();
    }

    public static SummativeDataLog Load()
    {
        string json = PlayerPrefs.GetString("SummativeDataLogs", "");

        if (string.IsNullOrEmpty(json))
            return new SummativeDataLog();

        return FromJson(json);
    }
}
