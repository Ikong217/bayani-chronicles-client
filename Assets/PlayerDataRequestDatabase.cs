using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class PlayerDataRequestDatabase : MonoBehaviour
{
    // Events
    public static event Action<string> OnDataLoadFailed;
    public static event Action<SimpleResponse> OnDataLoadSuccess;
    public static PlayerDataRequestDatabase Instance;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); // optional: keep across scenes
    }

    private void Start()
    {
        if (!ProgressData.IsSaved())
        {
            SavePlayerData();
            //print(PlayerLevelsData.LevelsData().JsonGetAll());
        }
    }

    public void SavePlayerData()
    {
        string id = MyData.Load().user_id;

        // scrollItem is already a List or array, no need to deserialize
        string scrollsObj = ScrollInventoryContainer.LoadData().ToJson();

        // Levels is JSON string, convert to object
        string levelsObj = PlayerLevelsData.LevelsData().JsonGetAll();
        string summativeObj = SummativeDataLog.Load().ToJson();

        StartCoroutine(HandleDataRequest(id, scrollsObj, levelsObj, summativeObj));
    }

    private IEnumerator HandleDataRequest(string id, string scrolls, string levels, string summative)
    {
        var payload = new
        {
            id = id,
            scrolls = scrolls,
            levels = levels,
            summative = summative
        };

        string jsonData = JsonConvert.SerializeObject(payload);
        string uri = LaravelRequest.GetLink("/player/data/request");

        using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                //Debug.Log("HTTP Response Code: " + www.responseCode);
                HandleErrorResponse(www);
            }
            else
            {
                HandleSuccessResponse(www);
            }
        }
    }

    private void HandleErrorResponse(UnityWebRequest www)
    {
        string jsonText = www.downloadHandler.text;
        Debug.LogWarning("Server Error Response: " + jsonText);

        try
        {
            SimpleResponse response = JsonConvert.DeserializeObject<SimpleResponse>(jsonText);
            string errorMessage = response?.message ?? www.error;
            OnDataLoadFailed?.Invoke(errorMessage);
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error (Error Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    private void HandleSuccessResponse(UnityWebRequest www)
    {
        string jsonText = www.downloadHandler.text;

        try
        {
            SimpleResponse response = JsonConvert.DeserializeObject<SimpleResponse>(jsonText);

            if (response != null && response.status == "success")
            {
                OnDataLoadSuccess?.Invoke(response);
                ProgressData.Save();
            }
            else
            {
                OnDataLoadFailed?.Invoke(response?.message ?? "Invalid server response.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error (Success Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }
}

/// <summary>
/// Matches your Laravel response:
/// { "status": "success", "message": "Data successfully updated" }
/// </summary>
[Serializable]
public class SimpleResponse
{
    public string status;
    public string message;
    public MyData user;
    public bool isBanned;
    public Dictionary<string, List<string>> errors;
}

public static class ProgressData
{
    // Returns true if saved, false otherwise
    public static bool IsSaved() => PlayerPrefs.GetInt("PlayerProgressSaved", 0) == 1;

    // Mark progress as altered (unsaved)
    public static void Altered() => PlayerPrefs.SetInt("PlayerProgressSaved", 0);

    // Mark progress as saved
    public static void Save() => PlayerPrefs.SetInt("PlayerProgressSaved", 1);
}
