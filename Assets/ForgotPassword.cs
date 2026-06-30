using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;
using com.ondad.alertpanels;

public class ForgotPassword : MonoBehaviour
{
    
    public GameObject getEmail;
    public GameObject codeVerification;
    public GameObject changePassword;

    private bool isAllowed = true;
    private bool ongoingRequest = false;

    private ExpectedResponse sessionResponse;
    private FpassState currentState = FpassState.request;
    public string email;
    public string code;
    private void Start()
    {
        Deactivate(); // hide everything at start
    }

    public string GetSessionResponseMessage()
    {
        return sessionResponse.message;
    }

    public string GetSessionResponseUrl()
    {
        return sessionResponse.uri;
    }

    public void OpenFpassPanel()
    {
        ActivatePanel(currentState);
    }

    public void Reset()
    {
        email = "";
        code = "";
        sessionResponse = new ExpectedResponse();
        currentState = FpassState.request;
        Deactivate();
    }

    public void PrinErrors(Action action = null)
    {
        if (sessionResponse != null && sessionResponse.status == "error")
        {
            string errorMessage = sessionResponse.message;

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
    }

    public void PrintMessage(string message, Action action = null)
    {
        AlertManager.GetInstance().ShowInfoPanel(message, action);
    }

    public void PrintWarningMessage(string message, Action action = null)
    {
        AlertManager.GetInstance().ShowWarningPanel(message, action);
    }

    public void PrintConfirmation(string message, Action exit = null, Action okay = null, Action cancel = null)
    {
        AlertManager.GetInstance().ShowConfirmationPanel(message, exit, okay, cancel);
    }

    public void ActivatePanel(FpassState state)
    {
        
        getEmail.SetActive(state == FpassState.request);
        codeVerification.SetActive(state == FpassState.verify);
        changePassword.SetActive(state == FpassState.reset);

        currentState = state;
    }

    public void Deactivate()
    {
        
        getEmail.SetActive(false);
        codeVerification.SetActive(false);
        changePassword.SetActive(false);
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
            Debug.Log("Action unavailable yet");
        }
    }

    private IEnumerator AllowInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isAllowed = true;
    }

    // ====== NETWORK REQUEST HANDLER ======
    public static event Action<string> OnDataLoadFailed;
    public static event Action<ExpectedResponse> OnDataLoadSuccess;

    public void FormulateRequest(object data, string url, Action successAction, Action failedAction)
    {
        print("Request Successfully sent");
        if (!ongoingRequest)
        {
            StartCoroutine(HandleDataRequest(data, url, successAction, failedAction));
        }
        else
        {
            AlertManager.GetInstance().ShowWarningPanel("Your request is already started, please wait a few moments.");
        }
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
            print("response Recireved");    

            if (www.result != UnityWebRequest.Result.Success)
                HandleErrorResponse(www, failedAction);
            else
                HandleSuccessResponse(www, successAction,failedAction);
        }

        ongoingRequest = false;
    }

    private void HandleErrorResponse(UnityWebRequest www, Action action)
    {
        string jsonText = www.downloadHandler.text;
        Debug.LogWarning("Server Error Response: " + jsonText);

        try
        {
            sessionResponse = JsonConvert.DeserializeObject<ExpectedResponse>(jsonText);
            string errorMessage = sessionResponse?.message ?? www.error;
            action?.Invoke();
            print("action called failed");
            OnDataLoadFailed?.Invoke(errorMessage);
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Parse Error (Error Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }

    private void HandleSuccessResponse(UnityWebRequest www, Action success, Action failed)
    {
        string jsonText = www.downloadHandler.text;

        try
        {
            sessionResponse = JsonConvert.DeserializeObject<ExpectedResponse>(jsonText);

            if (sessionResponse != null && sessionResponse.status == "success")
            {
                OnDataLoadSuccess?.Invoke(sessionResponse);
                if (sessionResponse.uri != null) code = sessionResponse.uri;
                success?.Invoke();
            }
            else
            {
                failed?.Invoke();
                OnDataLoadFailed?.Invoke(sessionResponse?.message ?? "Invalid server response.");
            }
        }
        catch (Exception e)
        {
            //failed?.Invoke();
            Debug.LogError("JSON Parse Error (Success Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }
}

// ====== SUPPORT CLASSES ======
[Serializable]
public class ExpectedResponse
{
    public string status;
    public string message;
    public Dictionary<string, List<string>> errors;
    public string uri;
}

public enum FpassState
{
    request,
    verify,
    reset
}
