using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;
//using com.ondad.alertpanels;

public class ScoreRequestHandler : MonoBehaviour
{
    public static ScoreRequestHandler Instance;
    private QuestionsRequestHandler qrHandler;

    private Novels novel;
    private Levels level;
    private string levelGetterString;

    public static event Action<string> OnScoreLoadFailed;
    public static event Action<LevelData> OnScoreLoadSuccess;
    public bool externalRequest = false;
    public bool externalSuccess = false;
    public bool allowRequest = true;
    private bool existingActivePlay = false;
    //private int tries;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        qrHandler = gameObject.GetComponent<QuestionsRequestHandler>();
        if (qrHandler == null)
            Debug.LogError("Missing component QuestionsRequestHandler");
        //print(MyData.Load().user_id);
        externalRequest = false;
    }

    private void Start()
    {
        novel = qrHandler.novel;
        level = qrHandler.level;
        //Debug.Log(LevelData.GetID(novel, level));
        if (!string.IsNullOrEmpty(LevelData.GetID(novel, level)))
        {
            existingActivePlay = true;
            if(allowRequest)
                StartCoroutine(StartRequestScoreUpdate(LevelData.GetID(novel, level), GameLevelPlayedStatus.Abandoned, 0));
        }
        else
        {
            if(allowRequest)
                StartCoroutine(StartRequestScoreAccess(novel, level));
        }
        //StartCoroutine(StartRequestScoreAccess(novel, level));
    }
    public void StartRequestingScoreUpdate(GameLevelPlayedStatus status, int score)
    {
        if(allowRequest)
            StartCoroutine(StartRequestScoreUpdate(LevelData.GetID(novel, level), status, score));
    }
    public void StartRequestingScoreAccess()
    {
        if (allowRequest)
            StartCoroutine(StartRequestScoreAccess(novel, level));
    }
    IEnumerator StartRequestScoreUpdate(string id, GameLevelPlayedStatus status, int score)
    {
        externalSuccess = false;
        externalRequest = true;
        var data = new
        {
            id = id,
            status = EnumHelper.GetLevelPlayedStatus(status),
            score = score
        };

        if (string.IsNullOrEmpty(id))
            Debug.LogWarning("There is no active Level");

        string jsonData = JsonConvert.SerializeObject(data);

        string uri = LaravelRequest.GetLink("/score/update");

        using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                HandleScoreErrorResponse(www);
                externalSuccess = false;
            }
            else
            {
                HandleSuccesPlayerUpdateScore(www);
                externalSuccess = true;
            }
        }
        if(!(status == GameLevelPlayedStatus.Completed || status == GameLevelPlayedStatus.Abandoned || status == GameLevelPlayedStatus.Quit || status == GameLevelPlayedStatus.Failed))
        {
            //print("got here");
            //print(status);
            //print(status != GameLevelPlayedStatus.Abandoned);
            StartCoroutine(StartRequestScoreAccess(novel, level));
        }

        if (existingActivePlay)
        {
            //print("and Here;");
            existingActivePlay = false;
            StartCoroutine(StartRequestScoreAccess(novel, level));
        }

        if(externalSuccess)
            LevelData.ClearData(novel, level);
        externalRequest = false;
    }

    private void HandleSuccesPlayerUpdateScore(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            //.Log("Server Response: " + jsonText);
            //print(jsonText);

            logResponseSuccess response = JsonConvert.DeserializeObject<logResponseSuccess>(jsonText);

            if (response != null && response.status == "success")
            {
                Debug.Log("Message");
            }
            else
            {
                Debug.LogError("Invalid response format");
                OnScoreLoadFailed?.Invoke("Invalid response format");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON Parse Error: {e.Message}");
            OnScoreLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    IEnumerator StartRequestScoreAccess(Novels requestNovel, Levels requestLevel)
    {
        var data = new
        {
            user_id = MyData.Load().user_id,
            novel = EnumHelper.GetNovel(requestNovel),
            level = EnumHelper.GetLevel(requestLevel),
        };

        if (string.IsNullOrEmpty(data.user_id))
        {
            Debug.LogWarning("You must need to Login Again");
            SceneManager.LoadScene("LoginScene");
        }

        string jsonData = JsonConvert.SerializeObject(data);

        string uri = LaravelRequest.GetLink("/score/start");

        using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                HandleScoreErrorResponse(www);
            }
            else
            {
                HandleSuccessGetPlayerScoreData(www);
            }
        }
    }

    private void HandleScoreErrorResponse(UnityWebRequest www)
    {
        try
        {

            string jsonText = www.downloadHandler.text;
            Debug.Log("Server Response (Error): " + jsonText);

            LogResponseData response = JsonConvert.DeserializeObject<LogResponseData>(jsonText);

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
                OnScoreLoadFailed?.Invoke(errorMessage);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
                print(www.error);
                OnScoreLoadFailed?.Invoke(www.error);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
            print("Server returned invalid data!");
            OnScoreLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    private void HandleSuccessGetPlayerScoreData(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            //.Log("Server Response: " + jsonText);

            LevelData response = JsonConvert.DeserializeObject<LevelData>(jsonText);
            //print(response);

            if (response != null && response.status == "success" && !string.IsNullOrEmpty(response.id))
            {
                response.LevelContainerStringify(novel, level);
                response.Save();
                //print(LevelData.GetID(novel, level));
                //print("succesful");

                // Notify subscribers that score data is loaded successfully
                OnScoreLoadSuccess?.Invoke(response);
            }
            else
            {
                Debug.LogError("Invalid response format");
                OnScoreLoadFailed?.Invoke("Invalid response format");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON Parse Error: {e.Message}");
            OnScoreLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }
}

public class logResponseSuccess
{
    public string status;
    public string message;
}

[System.Serializable]
public class LevelData
{
    public string id;
    public string status;
    public string levelContainer;

    public void Save()
    {
        if (!string.IsNullOrEmpty(levelContainer))
        {
            if (!string.IsNullOrEmpty(id))
                PlayerPrefs.SetString(levelContainer, id);
        }
        else
        {
            Debug.LogError("Please initialize the level first using LevelContainerStringify(Novels novel, Levels level)");
        }
    }
    public static void ClearData(Novels novel, Levels level)
    {
        string container = EnumHelper.GetNovel(novel) + "_" + EnumHelper.GetLevel(level);
        PlayerPrefs.DeleteKey(container);
    }
    public string LevelContainerStringify(Novels novel, Levels level)
    {
        levelContainer = EnumHelper.GetNovel(novel) + "_" + EnumHelper.GetLevel(level);
        return levelContainer;
    }

    // Fixed static method to get ID
    public static string GetID(Novels novel, Levels level)
    {
        string container = EnumHelper.GetNovel(novel) + "_" + EnumHelper.GetLevel(level);
        return PlayerPrefs.GetString(container, "");
    }
}

[System.Serializable]
public class LogResponseData
{
    public string status;
    public string message;
    public Dictionary<string, List<string>> errors;
}

public enum GameLevelPlayedStatus
{
    [Description("Ongoing")]
    Ongoing,
    [Description("Completed")]
    Completed,
    [Description("Restart")]
    Restart,
    [Description("Abandoned")]
    Abandoned,
    [Description("Failed")]
    Failed,
    [Description("Quit")]
    Quit,
}