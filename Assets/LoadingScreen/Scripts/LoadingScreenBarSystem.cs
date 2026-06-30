using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;
using TMPro;

public class LoadingScreenBarSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject bar;
    public Text loadingText;
    public GameObject BannedPanel;
    public TextMeshProUGUI message;

    private bool isDone = false;
    private bool isBanned = false;
    private bool hasUser = true;
    private float currentPercentage = 0f;
    private AsyncOperation async;

    public static event Action<string> OnDataLoadFailed;
    public static event Action<SimpleResponse> OnDataLoadSuccess;

    void Start()
    {
        StartCoroutine(StartLoading());
    }

    private IEnumerator StartLoading()
    {
        // Wait until internet is available
        while (!InternetChecker.HasInternetConnection())
        {
            Debug.LogWarning("⚠ No Internet Connection. Retrying in 2 seconds...");
            yield return new WaitForSeconds(2f);
        }

        string playerID = PlayerPrefs.GetString("user_id");
        Debug.Log("Player ID: " + playerID);

        yield return AnimateProgressTo(0.3f);

        if (!string.IsNullOrEmpty(playerID))
        {
            yield return AnimateProgressTo(0.8f);

            // Request access from server
            yield return StartCoroutine(RequestGetAccess(playerID));

            if (isBanned)
            {
                BannedPanel.SetActive(true);
                message.text = "You are banned. Please contact your teacher to update your status.";
                yield break;
            }

            if (!hasUser)
            {
                BannedPanel.SetActive(true);
                message.text = "Your account may have been deleted. Please contact your teacher for more information.";
                yield break;
            }

            // Load main menu
            yield return StartCoroutine(LoadSceneAsync(2));
        }
        else
        {
            // Load login/register scene
            yield return StartCoroutine(LoadSceneAsync(1));
        }
    }

    private IEnumerator AnimateProgressTo(float target)
    {
        while (currentPercentage < target)
        {
            currentPercentage = Mathf.MoveTowards(currentPercentage, target, Time.deltaTime * 0.5f);
            UpdateProgressUI();
            yield return null;
        }
    }

    private void UpdateProgressUI()
    {
        int percentage = Mathf.RoundToInt(currentPercentage * 100);

        if (loadingText != null)
            loadingText.text = percentage + "%";

        if (bar != null)
            bar.transform.localScale = new Vector3(currentPercentage, 1, 1);
    }

    private IEnumerator LoadSceneAsync(int sceneNo)
    {
        async = SceneManager.LoadSceneAsync(sceneNo);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            float targetProgress = Mathf.Clamp01(async.progress / 0.9f);
            currentPercentage = Mathf.MoveTowards(currentPercentage, targetProgress, Time.deltaTime * 0.5f);
            UpdateProgressUI();

            if (targetProgress >= 1f)
            {
                currentPercentage = 1f;
                UpdateProgressUI();
                yield return new WaitForSeconds(1f);
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator RequestGetAccess(string id)
    {
        var payload = new { id = id, reason = "game access" };
        string jsonData = JsonConvert.SerializeObject(payload);
        string uri = LaravelRequest.GetLink("/player/access/request");

        using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
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

            if(response.message == "User not found")
            {
                isDone = true;
                hasUser = false;
                MyData.ClearAll();
            }
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
        Debug.Log("✅ Server Response: " + jsonText);

        try
        {
            SimpleResponse response = JsonConvert.DeserializeObject<SimpleResponse>(jsonText);

            if (response != null && response.status == "success")
            {
                isDone = true;
                isBanned = response.isBanned;
                hasUser = true;

                OnDataLoadSuccess?.Invoke(response);

                if (response.user != null)
                {
                    response.user.SaveAll();
                    print(response.user.username);
                }
                else
                {
                    Debug.LogWarning("⚠ No user data in response!");
                    hasUser = false;
                }
            }
            else if(response.status == "error")
            {
                isDone = true;
                hasUser = false;
                print("deleted user");
                Debug.LogWarning("⚠ Response status not 'success': " + response?.status);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error (Success Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }
}
public static class InternetChecker
{
    public static bool HasInternetConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
