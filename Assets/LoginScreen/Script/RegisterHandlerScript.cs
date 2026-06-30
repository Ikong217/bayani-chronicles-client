using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using com.ondad.alertpanels;


public class LoginHandlerScript : MonoBehaviour
{
    public InputField username;
    public InputField email;
    public InputField password;
    public InputField confirm_pass;
    public Canvas canvasAlertPanel;

    public GameObject loginCanvas;
    public GameObject registerCanvas;

    string insertURL = "http://127.0.0.1:8000/api/user/insert";

    public void printWords()
    {
        StartCoroutine(SendData(username.text, email.text, password.text, confirm_pass.text));
    }

    IEnumerator SendData(string username, string email, string password, string confirm_pass)
    {
        string jsonData = JsonUtility.ToJson(new UserData(username, email, password, confirm_pass));

        using (UnityWebRequest www = new UnityWebRequest(insertURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonText = www.downloadHandler.text;
                    Debug.Log("Server Response: " + jsonText);
                    ResponseData response = JsonConvert.DeserializeObject<ResponseData>(jsonText);

                    if (response != null && response.status == "error")
                    {
                        string errorMessage = response.message;

                        if (response.errors != null && response.errors.Count > 0)
                        {
                            errorMessage += "\n";
                            foreach (var error in response.errors)
                            {
                                foreach (string errorMsg in error.Value) // 🔥 Loop through error messages correctly
                                {
                                    errorMessage += "- " + errorMsg + "  ";
                                }
                            }
                        }

                    AlertManager.GetInstance().ShowErrorPanel(errorMessage);
                    }
                    else
                    {
                        //Debug.LogError("Error: " + www.error);
                    AlertManager.GetInstance().ShowErrorPanel("Error: " + www.error);
                    }
                }
                catch (System.Exception )
                {
                    //Debug.LogError("JSON Parse Error: " + e.Message);
                AlertManager.GetInstance().ShowErrorPanel("Server returned invalid data!");
                }
            }
            else
            {
                string jsonText = www.downloadHandler.text;
                Debug.Log("Server Response: " + jsonText);

                try
                {
                    ResponseData response = JsonUtility.FromJson<ResponseData>(jsonText);

                    if (response != null && response.status == "success")
                    {
                    AlertManager.GetInstance().ShowInfoPanel("Your account " + response.user.email + " has been created", okeyAction: toLogin);

                        this.username.text = "";
                        this.email.text = "";
                        this.password.text = "";
                        this.confirm_pass.text = "";
                        this.confirm_pass.text = "";
                    }
                    else
                    {
                    AlertManager.GetInstance().ShowErrorPanel(response != null ? response.message : "Invalid JSON response");
                    }
                }
                catch (System.Exception )
                {
                    //Debug.LogError("JSON Parse Error: " + e.Message);
                AlertManager.GetInstance().ShowErrorPanel("Server returned invalid data!");
                }
            }
            refreshAlert();
        }
    }

    public void refreshAlert()
    {
        canvasAlertPanel.enabled = false;
        canvasAlertPanel.enabled = true;
    }

    public void toLogin()
    {
        loginCanvas.SetActive(true);
        registerCanvas.SetActive(false);
    }
}

[System.Serializable]
public class ResponseData
{
    public string status;
    public string message;
    public Dictionary<string, string[]> errors;
    public UserData user;
}

[System.Serializable]
public class UserData
{
    public string username;
    public string email;
    public string password;
    public string password_confirmation;

    public UserData(string username, string email, string password, string password_confirmation)
    {
        this.username = username;
        this.email = email;
        this.password = password;
        this.password_confirmation = password_confirmation;
    }
}