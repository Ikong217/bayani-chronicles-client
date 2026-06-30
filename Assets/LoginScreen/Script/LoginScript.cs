using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using com.ondad.alertpanels;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class LoginScript : MonoBehaviour
{
    public InputField email;
    public InputField password;
    public Canvas canvasAlertPanel;
    public GameObject otpPanel;
    public Toggle tnc;

    public void login()
    {
        if(tnc != null && tnc.isOn)
        {
            TMP_InputField emailTxt = null;
            GameObject emailObj = email.gameObject;
            Transform emailTxtTransform = emailObj.transform.Find("Text");
            if (emailTxtTransform != null)
            {
                GameObject emailTxtObj = emailTxtTransform.gameObject;
                emailTxt = emailTxtObj.GetComponent<TMP_InputField>();
            }

            TMP_InputField passTxt = null;
            GameObject passObj = password.gameObject;
            Transform passTxtTransform = passObj.transform.Find("Text");
            if (passTxtTransform != null)
            {
                GameObject passTxtObj = passTxtTransform.gameObject;
                passTxt = passTxtObj.GetComponent<TMP_InputField>();
            }

            StartCoroutine(SendData(emailTxt.text, passTxt.text));
        }
        else
        {
            AlertManager.GetInstance().ShowErrorPanel("You must Agree to the Terms and Condition before Logging in");
        }
    }

    IEnumerator SendData(string email, string password)
    {
        logUserData logUserData = new logUserData(email, password);
        string jsonData = JsonConvert.SerializeObject(logUserData);
        string insertURL = LaravelRequest.GetLink("/api/user/login");

        using (UnityWebRequest www = new UnityWebRequest(insertURL, "POST"))
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

        refreshAlert();
    }

    void HandleErrorResponse(UnityWebRequest www)
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

                AlertManager.GetInstance().ShowWarningPanel(errorMessage);
            }
            else
            {
                Debug.LogWarning("Error: " + www.error);
                Debug.Log("Retrying to Login");
                login();
                AlertManager.GetInstance().ShowErrorPanel("Error: " + www.error);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
            AlertManager.GetInstance().ShowErrorPanel("Server returned invalid data!");
        }
    }

    void HandleSuccessResponse(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            Debug.Log("Server Response (Success): " + jsonText);

            logResponseData response = JsonConvert.DeserializeObject<logResponseData>(jsonText);

            if (response != null && response.status == "success")
            {
                OtpResponseData otpResponse = new OtpResponseData();
                otpResponse.status = response.status;
                otpResponse.message = response.message;
                otpResponse.otp_key = response.otp_key;

                OtpKey.saveKey(otpResponse.otp_key);
                otpPanel.SetActive(true);
            }
            else
            {
                AlertManager.GetInstance().ShowErrorPanel(response != null ? response.message : "Invalid JSON response");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
            AlertManager.GetInstance().ShowErrorPanel("Server returned invalid data!");
        }
    }

    public void refreshAlert()
    {
        canvasAlertPanel.enabled = false;
        canvasAlertPanel.enabled = true;
    }
}

[System.Serializable]
public class logResponseData
{
    public string status;
    public string message;
    public string otp_key;
    public string encID;
    public MyData user;
    public string levels;
    public string scrolls;
    public string summative;
    public Dictionary<string, List<string>> errors;
}

[System.Serializable]
public class OtpResponseData
{
    public string status;
    public string message;
    public string otp_key;
}

public static class OtpKey
{
    public static Action<string> saveKey = SaveKey;
    public static Func<string> getKey = GetKey;

    private static string GetKey()
    {
        return PlayerPrefs.GetString("otpKey", "");
    }

    private static void SaveKey(string key)
    {
        PlayerPrefs.SetString("otpKey", key);
        PlayerPrefs.Save();
    }
}