using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;

public class QuestionsRequestHandler : MonoBehaviour
{
    public static QuestionsRequestHandler Instance;
    public QuestionsContainer questions;
    public Novels novel;
    public Levels level;

    // Add events to notify when questions are loaded
    public static event Action<QuestionsContainer> OnQuestionsLoaded;
    public static event Action<string> OnQuestionsLoadFailed;

    private void Awake() // Changed from Start to Awake
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Only start the request if this is the singleton instance
        if (Instance == this)
        {
            StartCoroutine(HandleQuestionRequestData(this.novel, this.level));
        }

    }

    // Make this public so it can be called from other scripts
    public void RequestQuestions(Novels novel, Levels level)
    {
        this.novel = novel;
        this.level = level;
        StartCoroutine(HandleQuestionRequestData(novel, level));
    }

    IEnumerator HandleQuestionRequestData(Novels novel, Levels level)
    {
        string novelStr = EnumHelper.GetNovel(novel);
        string levelStr = EnumHelper.GetLevel(level);

        CurrentLevel current = new CurrentLevel(novelStr, levelStr);
        string jsonData = JsonConvert.SerializeObject(current);

        string uri = LaravelRequest.GetLink("/questions/request");

        // DEBUG EVERYTHING
        //Debug.Log($"URL: {uri}");
        using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            //Debug.Log($"Result: {www.result}");
            //Debug.Log($"Response Code: {www.responseCode}");
            //Debug.Log($"Error: {www.error}");

            if (www.result != UnityWebRequest.Result.Success)
            {
                HandleQuestionsErrorResponse(www);
                OnQuestionsLoadFailed?.Invoke(www.error);
            }
            else
            {
                HandleQuestionsSuccessResponse(www);
            }
        }
    }

    void HandleQuestionsErrorResponse(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            Debug.Log("Server Response (Error): " + jsonText);

            logResponseData response = JsonConvert.DeserializeObject<logResponseData>(jsonText);

            if (response != null && response.status == "error")
            {
                string errorMessage = response.message;

                if (response.errors != null && response.errors.Count > 0)
                {
                    errorMessage += "\n";
                    foreach (var error in response.errors)
                    {
                        foreach (string errorMsg in error.Value)
                        {
                            errorMessage += "- " + errorMsg + "  ";
                        }
                    }
                }

                print(errorMessage);
                OnQuestionsLoadFailed?.Invoke(errorMessage);
            }
            else
            {
                Debug.LogWarning("Error: " + www.error);
                Debug.LogWarning("no available question, trying Again");
                RequestQuestions(this.novel, this.level);
                //print(www.error);
                OnQuestionsLoadFailed?.Invoke(www.error);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
            print("Server returned invalid data!");
            OnQuestionsLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    void HandleQuestionsSuccessResponse(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            //Debug.Log("Server Response: " + jsonText);

            QuestionsResponse response = JsonConvert.DeserializeObject<QuestionsResponse>(jsonText);

            if (response != null && response.status == "success" && response.questions != null)
            {
                questions = new QuestionsContainer();
                questions.questions = response.questions;
                print(questions.questions.Count);

                //Debug.Log($"Successfully loaded {questions.questions.Count} questions");

                // Notify subscribers that questions are loaded
                OnQuestionsLoaded?.Invoke(questions);

                // Example: Print first question
                //if (questions.questions.Count > 0)
                //{
                //    var firstQuestion = questions.questions[0];
                //    Debug.Log($"Q: {firstQuestion.question}");
                //    Debug.Log($"A: {firstQuestion.answer}");
                //}
            }
            else
            {
                Debug.LogError("Invalid response format");
                OnQuestionsLoadFailed?.Invoke("Invalid response format");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON Parse Error: {e.Message}");
            OnQuestionsLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }
}

// Fixed response class structure to match what the server might return
[System.Serializable]
public class QuestionsStorage
{
    public int id;
    public string type;
    public string question;
    public string answer;
    public string[] otherAnswers;
    public string rationalization;
}

[System.Serializable]
public class QuestionsResponse
{
    public string status;
    public List<QuestionsStorage> questions;
}

[System.Serializable]
public class QuestionsContainer
{
    public List<QuestionsStorage> questions = new List<QuestionsStorage>();
}

public class CurrentLevel
{
    public string novel;
    public string level;

    public CurrentLevel(string novel, string level)
    {
        this.novel = novel;
        this.level = level;
    }
}

public enum Novels
{
    [Description("Noli Me Tangere")]
    NoliMeTangere,

    [Description("El Filibusterismo")]
    ElFilibusterismo,
}

public enum Levels
{
    [Description("level 1")]
    Level1,
    [Description("level 2")]
    Level2,
    [Description("level 3")]
    Level3,
    [Description("level 4")]
    Level4,
    [Description("level 5")]
    Level5,
    [Description("level 6")]
    Level6,
    [Description("level 7")]
    Level7,
    [Description("level 8")]
    Level8,
    [Description("level 9")]
    Level9,
    [Description("level 10")]
    Level10,
}

public static class EnumHelper
{
    public static string GetNovel(Novels novel)
    {
        var field = novel.GetType().GetField(novel.ToString());
        var attribute = (DescriptionAttribute)System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute == null ? novel.ToString() : attribute.Description;
    }

    public static string GetLevel(Levels level)
    {
        var field = level.GetType().GetField(level.ToString());
        var attribute = (DescriptionAttribute)System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute == null ? level.ToString() : attribute.Description;
    }

    public static string GetLevelPlayedStatus(GameLevelPlayedStatus gamePlayedStatus)
    {
        var field = gamePlayedStatus.GetType().GetField(gamePlayedStatus.ToString());
        var attribute = (DescriptionAttribute)System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute == null ? gamePlayedStatus.ToString() : attribute.Description;
    }
}