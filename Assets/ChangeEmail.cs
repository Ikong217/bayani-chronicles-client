using UnityEngine;
using TMPro;
using System.Collections;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using com.ondad.alertpanels;

public class ChangeEmail : MonoBehaviour
{
    public Phase phase = Phase.GetEmail;

    [SerializeField] private GameObject ChEm;
    [SerializeField] private GameObject OldEm;
    [SerializeField] private GameObject NewEm;
    [SerializeField] private MainMenu mm;

    private bool isAllowed = true;
    private bool ongoingRequest = false;
    private NewEmailResponse sessionResponse;

    // ===============================
    // PANEL HANDLER
    // ===============================
    private void OnEnable()
    {
        mm.BoxOutButtonPressed();
        ActivatePanel(Phase.GetEmail);
    }

    public void ActivatePanel(Phase phase)
    {
        //Reset(); // Reset states before activation

        switch (phase)
        {
            case Phase.GetEmail:
                ChEm.SetActive(true);
                break;
            case Phase.OldCode:
                OldEm.SetActive(true);
                break;
            case Phase.NewCode:
                NewEm.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown phase: " + phase);
                break;
        }

        this.phase = phase;
    }

    public void Reset()
    {
        phase = Phase.GetEmail;
        ChEm.SetActive(false);
        OldEm.SetActive(false);
        NewEm.SetActive(false);

        isAllowed = true;
        sessionResponse = null;
        ongoingRequest = false;
        mm.ProfileButtonPressed();
    }

    public void Disable() => gameObject.SetActive(false);

    // ===============================
    // ALERT HANDLERS
    // ===============================
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

            AlertManager.GetInstance().ShowWarningPanel(errorMessage, action);
        }
        else
        {
            Debug.Log("No errors to print.");
        }
    }
    public void Quit()
    {
        Reset();
        Disable();
    }

    public string GetMessage() => sessionResponse.message;
    public string GetCode() => sessionResponse.code;

    public MyData GetMyData() => sessionResponse.user;

    public void PrintMessage(string message, Action action = null)
        => AlertManager.GetInstance().ShowInfoPanel(message, action);

    public void PrintWarningMessage(string message, Action action = null)
        => AlertManager.GetInstance().ShowWarningPanel(message, action);

    public void PrintConfirmation(string message, Action exit = null, Action okay = null, Action cancel = null)
        => AlertManager.GetInstance().ShowConfirmationPanel(message, exit, okay, cancel);

    // ===============================
    // ACTION CONTROL
    // ===============================
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
            Debug.Log("Action unavailable yet. Please wait...");
        }
    }

    private IEnumerator AllowInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isAllowed = true;
    }

    // ===============================
    // NETWORK REQUEST HANDLER
    // ===============================
    public static event Action<string> OnDataLoadFailed;
    public static event Action<NewEmailResponse> OnDataLoadSuccess;

    public void FormulateRequest(object data, string url, Action successAction, Action failedAction)
    {
        if (ongoingRequest)
        {
            AlertManager.GetInstance().ShowWarningPanel("Your request is already in progress, please wait.");
            return;
        }

        Debug.Log("Sending request...");
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

            Debug.Log("Response received");

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
            sessionResponse = JsonConvert.DeserializeObject<NewEmailResponse>(jsonText);
            string errorMessage = sessionResponse?.message ?? www.error ?? "Unknown error.";

            failedAction?.Invoke();
            OnDataLoadFailed?.Invoke(errorMessage);

            Debug.Log("Error handled successfully.");
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
            sessionResponse = JsonConvert.DeserializeObject<NewEmailResponse>(jsonText);

            if (sessionResponse != null && sessionResponse.status == "success")
            {
                OnDataLoadSuccess?.Invoke(sessionResponse);
                successAction?.Invoke();
            }
            else
            {
                failedAction?.Invoke();
                OnDataLoadFailed?.Invoke(sessionResponse?.message ?? "Invalid server response.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error (Success Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    // ===============================
    // ENUM
    // ===============================
    public enum Phase
    {
        GetEmail,
        OldCode,
        NewCode
    }
}
public class NewEmailResponse
{
    public string status;
    public string message;
    public Dictionary<string, List<string>> errors;
    public string code;
    public MyData user;
}