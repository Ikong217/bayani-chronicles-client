using UnityEngine;
using TMPro;
using System.Collections;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;
using com.ondad.alertpanels;

public class ChangeUsername : MonoBehaviour
{
    [SerializeField] private GameObject getUsernamePanel;
    [SerializeField] private GameObject getPasswordPanel;

    [SerializeField] private TMP_InputField usernameInp;
    [SerializeField] private TMP_InputField passInp;

    [SerializeField] private MainMenu mm;

    private string username = "";
    private string password = "";

    private string userID;

    private logResponseData sessionResponse;
    private bool isAllowed = true;
    private bool ongoingRequest = false;

    private void Start()
    {
        userID = MyData.Load().user_id;
        Reset();
    }

    public void Reset()
    {
        usernameInp.text = passInp.text = username = password = "";
        isAllowed = true;
        ongoingRequest = false;
        getUsernamePanel.SetActive(false);
        getPasswordPanel.SetActive(false);
        gameObject.SetActive(false);
        mm.ProfileButtonPressed();
    }

    private void OnEnable()
    {
        ActivateGetUsername();
        userID = MyData.Load().user_id;
    }

    public void ActivateGetUsername()
    {
        getUsernamePanel.SetActive(true);
    }

    public void ActivateGetPassword()
    {
        getPasswordPanel.SetActive(true);
    }

    public void OnPressedExit()
    {
        Reset();
    }

    public void OnPressedNext()
    {
        string usernameTxt = usernameInp.text.Trim();
        if (string.IsNullOrEmpty(usernameTxt))
        {
            PrintWarningMessage("You Must Enter A Username");
            return;
        }else if(usernameTxt.Length < 8)
        {
            PrintWarningMessage("You must Make your username longer");
            return;
        }
        username = usernameTxt;
        ActivateGetPassword();
    }

    public void OnPressedSubmit()
    {
        DisableInSeconds(Submit, 2);
    }

    private void Submit()
    {
        string passwordTxt = passInp.text.Trim();
        if (string.IsNullOrEmpty(passwordTxt))
        {
            PrintWarningMessage("Please Enter Your Password");
            return;
        }
        else if (passwordTxt.Length < 8)
        {
            PrintWarningMessage("Please Use YOur Valid Password");
            return;
        }

        password = passwordTxt;

        string uri = "api/user/request/username/change";    

        var data = new
        {
            user_id = userID,
            username = username,
            password = password
        };

        FormulateRequest(data, uri, Success, Fail);
    }

    private void Success()
    {
        PrintMessage(sessionResponse.message, ()=> {
            MyData data = MyData.Load();
            data.username = sessionResponse.user.username;
            data.SaveAll();

            ShowProfileScript.Instance.UpdateUserData();
            Reset();
        });
    }

    private void Fail()
    {
        PrintErrors(AdditionalErrorProcess);
    }

    private void AdditionalErrorProcess()
    {
        if(sessionResponse.message == "Username Taken")
        {
            getPasswordPanel.SetActive(false);
            ActivateGetUsername();
        }else if(sessionResponse.message == "Username Recently Changed")
        {
            Reset();
        }
    }

    public void PrintErrors(Action action = null)
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
    public static event Action<logResponseData> OnDataLoadSuccess;

    public void FormulateRequest(object data, string url, Action successAction, Action failedAction)
    {
        Debug.Log("Request successfully sent");
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
            Debug.Log("Response received");

            if (www.result != UnityWebRequest.Result.Success)
                HandleErrorResponse(www, failedAction);
            else
                HandleSuccessResponse(www, successAction, failedAction);
        }

        ongoingRequest = false;
    }

    private void HandleErrorResponse(UnityWebRequest www, Action action)
    {
        string jsonText = www.downloadHandler.text;
        Debug.LogWarning("Server Error Response: " + jsonText);

        try
        {
            sessionResponse = JsonConvert.DeserializeObject<logResponseData>(jsonText);
            string errorMessage = sessionResponse?.message ?? www.error;
            action?.Invoke();
            Debug.Log("Action called (failed)");
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
            sessionResponse = JsonConvert.DeserializeObject<logResponseData>(jsonText);

            if (sessionResponse != null && sessionResponse.status == "success")
            {
                OnDataLoadSuccess?.Invoke(sessionResponse);
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
            Debug.LogError("JSON Parse Error (Success Response): " + e.Message);
            OnDataLoadFailed?.Invoke("JSON Parse Error: " + e.Message);
        }
    }
}
