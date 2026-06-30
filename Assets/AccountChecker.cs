using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;
using com.ondad.alertpanels;
using UnityEngine.SceneManagement;

public class AccountChecker : MonoBehaviour
{
    private SimpleResponse sessionResponse;
    private bool isAllowed = true;
    private bool ongoingRequest = false;

    private void Start()
    {
        StartRequest();
    }

    private void StartRequest()
    {
        string id = MyData.Load().user_id;

        if (string.IsNullOrEmpty(id))
        {
            PrintMessage("It seems that you have no User Data, Please Log In Again",RedirectLogin);
            return;
        }

        var data = new
        {
            id = id,
            reason = "",
        };
        string url = "/player/access/request";
        // ✅ Fixed: Added missing arguments (Success, Fail)
        FormulateRequest(data, url, Success, Fail);
    }

    private void Success()
    {
        if (sessionResponse == null)
        {
            PrintWarningMessage("Empty response received from the server.");
            return;
        }

        if (sessionResponse.isBanned)
        {
            PrintMessage("You have been banned. Please contact your teacher for more information.", RedirectLogin);
        }
        else
        {
            // ✅ Safe call for SaveAll()
            sessionResponse.user?.SaveAll();
            // ✅ Added float suffix
            StartCoroutine(WaitSeconds(StartRequest, 90f));
        }
    }

    private void Fail()
    {
        if (sessionResponse == null)
        {
            PrintWarningMessage("No response from server. Please try again later.");
            return;
        }

        if (sessionResponse.message == "User not found")
        {
            PrintMessage("Your data was probably deleted. Please contact your teacher for more information.");
            MyData.ClearAll();
        }
        else
        {
            PrintErrors();
        }
    }

    IEnumerator WaitSeconds(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    private void RedirectLogin()
    {
        SceneManager.LoadScene("LoginScene");
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
    // ✅ Fixed: Use SimpleResponse instead of LeaderboardResponse
    public static event Action<string> OnDataLoadFailed;
    public static event Action<SimpleResponse> OnDataLoadSuccess;

    // ====== REQUEST HANDLER ======
    public void FormulateRequest(object data, string url, Action successAction, Action failedAction)
    {
        if (ongoingRequest)
        {
            AlertManager.GetInstance()?.ShowWarningPanel("A request is already in progress. Please wait...");
            return;
        }

        Debug.Log($"Sending request to: {url}");
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
            Debug.Log("Response received from server.");

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
            sessionResponse = JsonConvert.DeserializeObject<SimpleResponse>(jsonText);
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

        try
        {
            sessionResponse = JsonConvert.DeserializeObject<SimpleResponse>(jsonText);
            if (sessionResponse != null && sessionResponse.status == "success")
            {
                OnDataLoadSuccess?.Invoke(sessionResponse);
                successAction?.Invoke();
                Debug.Log("Request succeeded.");
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
