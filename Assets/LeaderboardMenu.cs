using UnityEngine;
using TMPro;
using System.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using com.ondad.alertpanels;

public class LeaderboardMenu : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private LeaderboardItem currentPlayer;
    [SerializeField] private GameObject sourcePlayerItem;

    private LeaderboardResponse sessionResponse;
    private bool isAllowed = true;
    private bool ongoingRequest = false;

    private void OnEnable()
    {
        StartRequestLeaderboard();
    }

    public void StartRequestLeaderboard()
    {
        var data = new
        {
            user_id = MyData.Load().user_id // must be encrypted from Unity side before sending
        };

        string url = "/leaderboard/request"; // Laravel endpoint route

        FormulateRequest(data, url, Success, Fail);
    }

    private void Success()
    {
        if (sessionResponse?.leaderboard == null || sessionResponse.leaderboard.Count == 0)
        {
            PrintMessage("No leaderboard data available.");
            return;
        }

        sourcePlayerItem = Resources.Load<GameObject>("Player");

        // Remove all old children in the container
        foreach (Transform child in container.transform)
            Destroy(child.gameObject);

        // Load current player info from saved data
        var myData = MyData.Load();
        string myUsername = myData.username;
        string mySection = $"{myData.grade_lvl} - {myData.section_name}";

        LeaderboardItem myPlayerItem = null;

        // Loop through each leaderboard entry
        foreach (LeaderboardData leaderboard in sessionResponse.leaderboard)
        {
            // Instantiate new leaderboard row
            GameObject newItem = Instantiate(sourcePlayerItem, container.transform);

            // Determine color based on rank
            Color rankColor = new Color(0.25f, 0.25f, 0.25f); // Default dark gray
            if (int.TryParse(leaderboard.ranking, out int rank))
            {
                switch (rank)
                {
                    case 1: rankColor = new Color(0.90f, 0.65f, 0f); break; 
                    case 2: rankColor = new Color(0.49f, 0.54f, 0.59f); break; 
                    case 3: rankColor = new Color(0.0f, 0.5f, 0.39f); break;

                }
            }

            // Fetch LeaderboardItem component
            LeaderboardItem itemScript = newItem.GetComponent<LeaderboardItem>();
            if (itemScript != null)
            {
                itemScript.Init(
                    ranking: leaderboard.ranking,
                    username: leaderboard.username,
                    section: leaderboard.section,
                    stars: leaderboard.stars,
                    totalScore: leaderboard.total_score,
                    average: leaderboard.average,
                    attempts: leaderboard.attempts,
                    color: rankColor
                );
            }

            // Highlight the current player's entry
            if (leaderboard.username == myUsername)
            {
                Color me = Color.green;
                if (int.Parse(leaderboard.ranking) <= 3)
                {
                    me = rankColor;
                }
                // Make their background white (or another accent)
                itemScript.Init(
                    ranking: leaderboard.ranking,
                    username: leaderboard.username,
                    section: leaderboard.section,
                    stars: leaderboard.stars,
                    totalScore: leaderboard.total_score,
                    average: leaderboard.average,
                    attempts: leaderboard.attempts,
                    color: me
                );

                currentPlayer.Init(
                    ranking: leaderboard.ranking,
                    username: leaderboard.username,
                    section: leaderboard.section,
                    stars: leaderboard.stars,
                    totalScore: leaderboard.total_score,
                    average: leaderboard.average,
                    attempts: leaderboard.attempts,
                    color: me
                );
                
            }
        }

        // Re-request leaderboard every 10 seconds
        StartCoroutine(RefreshLeaderboard(10f));
    }

    private IEnumerator RefreshLeaderboard(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRequestLeaderboard();
    }

    private void Fail()
    {
        //print("nag error");
        PrintErrors();
    }

    // ====== ALERTS ======
    public void PrintErrors(Action action = null)
    {
        if (sessionResponse != null && sessionResponse.status == "error")
        {
            string errorMessage = sessionResponse.message ?? "An unknown error occurred.";

            if (sessionResponse.errors != null && sessionResponse.errors.Count > 0)
            {
                errorMessage += "\n";
                foreach (var error in sessionResponse.errors)
                {
                    foreach (string msg in error.Value)
                        errorMessage += "- " + msg + "  ";
                }
            }

            AlertManager.GetInstance()?.ShowWarningPanel(errorMessage, action);
        }
    }

    public void PrintMessage(string message, Action action = null)
    {
        AlertManager.GetInstance()?.ShowInfoPanel(message ?? "No message provided.", action);
    }

    public void PrintWarningMessage(string message, Action action = null)
    {
        AlertManager.GetInstance()?.ShowWarningPanel(message ?? "Warning: Something went wrong.", action);
    }

    public void PrintConfirmation(string message, Action exit = null, Action okay = null, Action cancel = null)
    {
        AlertManager.GetInstance()?.ShowConfirmationPanel(message ?? "Are you sure?", exit, okay, cancel);
    }

    // ====== TIMEOUT MECHANISM ======
    public void DisableInSeconds(Action action, float seconds)
    {
        if (isAllowed)
        {
            isAllowed = false;
            action?.Invoke();
            StartCoroutine(AllowInSeconds(seconds));
        }
        else
        {
            Debug.Log("Action unavailable yet.");
        }
    }

    private IEnumerator AllowInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isAllowed = true;
    }

    // ====== NETWORK EVENTS ======
    public static event Action<string> OnDataLoadFailed;
    public static event Action<LeaderboardResponse> OnDataLoadSuccess;

    // ====== REQUEST HANDLER ======
    public void FormulateRequest(object data, string url, Action successAction, Action failedAction)
    {
        if (ongoingRequest)
        {
            AlertManager.GetInstance()?.ShowWarningPanel("A request is already in progress. Please wait...");
            return;
        }

        Debug.Log($"Sending leaderboard request to: {url}");
        StartCoroutine(HandleDataRequest(data, url, successAction, failedAction));
    }

    private IEnumerator HandleDataRequest(object data, string uri, Action successAction, Action failedAction)
    {
        ongoingRequest = true;

        string jsonData = JsonConvert.SerializeObject(data);
        string url = LaravelRequest.GetLink(uri);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            Debug.Log("Leaderboard response received.");

            if (www.result != UnityWebRequest.Result.Success)
                HandleErrorResponse(www, failedAction);
            else
                HandleSuccessResponse(www, successAction, failedAction);
        }

        ongoingRequest = false;
    }

    private void HandleErrorResponse(UnityWebRequest www, Action failedAction)
    {
        string jsonText = www.downloadHandler.text;
        Debug.LogWarning("Server Error Response: " + jsonText);

        try
        {
            sessionResponse = JsonConvert.DeserializeObject<LeaderboardResponse>(jsonText);
            string errorMessage = sessionResponse?.message ?? www.error ?? "Unknown error";

            failedAction?.Invoke();
            OnDataLoadFailed?.Invoke(errorMessage);
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error (Error Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    private void HandleSuccessResponse(UnityWebRequest www, Action successAction, Action failedAction)
    {
        string jsonText = www.downloadHandler.text;
        //print(jsonText);

        try
        {
            sessionResponse = JsonConvert.DeserializeObject<LeaderboardResponse>(jsonText);
            if (sessionResponse != null && sessionResponse.status == "success")
            {
                OnDataLoadSuccess?.Invoke(sessionResponse);
                successAction?.Invoke();
                Debug.Log("Leaderboard request succeeded.");
            }
            else
            {
                failedAction?.Invoke();
                OnDataLoadFailed?.Invoke(sessionResponse?.message ?? "Invalid or incomplete server response.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error (Success Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }
}

// ====== RESPONSE CLASSES ======
[Serializable]
public class LeaderboardResponse
{
    public string status;
    public string message;
    public List<LeaderboardData> leaderboard;
    public Dictionary<string, List<string>> errors;
}

[Serializable]
public class LeaderboardData
{
    public string username;
    public string section;
    public string stars;
    public string total_score;
    public string average;
    public string attempts;
    public string ranking;

    // Numeric fallbacks
    public int Stars => int.TryParse(stars, out var s) ? s : 0;
    public int Attempts => int.TryParse(attempts, out var a) ? a : 0;
    public float Average => float.TryParse(average, out var avg) ? avg : 0f;

    public int TotalScore
    {
        get
        {
            // Extract value before "/" (e.g. "250/300" -> 250)
            if (string.IsNullOrEmpty(total_score)) return 0;
            var parts = total_score.Split('/');
            return int.TryParse(parts[0], out var val) ? val : 0;
        }
    }
}
