using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using com.ondad.alertpanels;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class otpScript : MonoBehaviour
{
    public Canvas canvasAlertPanel;
    public TextMeshProUGUI input;
    public Button verifyBtn;
    public GameObject otpPanel;

    [System.Serializable]
    public class VerifyOtpData
    {
        public string key;
        public int otp;
    }

    [System.Serializable]
    public class ResendOtpData
    {
        public string key;
    }

    private void Start()
    {
        //ValidateInput();
    }

    //public void ValidateInput()
    //{
    //    bool isValid = (input.text.Length == 6);
    //    verifyBtn.enabled = isValid;
    //}

    public void VerifyOtp()
    {
        string otpstr = new string(input.text.Where(char.IsDigit).ToArray());
        if (otpstr.Length != 6)
        {
            AlertManager.GetInstance().ShowWarningPanel("The Otp must contain 6 numbers \n-length = " + otpstr.Length);
            return;
        }
        // Remove ANY non-digit characters
        string cleanText = new string(input.text.Where(c => char.IsDigit(c)).ToArray());

        if (cleanText.Length == 6 && int.TryParse(cleanText, out int otp))
        {
            string key = OtpKey.getKey();
            StartCoroutine(StartRequestVerifyOtp(key, otp));
        }
        else
        {
            AlertManager.GetInstance().ShowWarningPanel("Please enter exactly 6 numbers");
        }
    }

    IEnumerator StartRequestVerifyOtp(string key, int otp)
    {
        VerifyOtpData data = new VerifyOtpData
        {
            key = key,
            otp = otp
        };

        string jsonData = JsonConvert.SerializeObject(data);
        string insertURL = LaravelRequest.GetLink("/api/user/verify-otp");

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
                HandleOtpValidationSuccessResponse(www);
            }
        }

        refreshAlert();
    }

    void HandleOtpValidationSuccessResponse(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            Debug.Log("Server Response (Success): " + jsonText);

            logResponseData response = JsonConvert.DeserializeObject<logResponseData>(jsonText);

            if (response != null && response.status == "success")
            {
                MyData data = response.user;
                data.SaveAll();

                
                if (!string.IsNullOrEmpty(response.levels))
                {
                    print(response.levels);
                    PlayerLevelsData playerLevelData = PlayerLevelsData.JsonConvertAll(response.levels);
                    PlayerLevelsData.SaveData(playerLevelData);

                }

                if (!string.IsNullOrEmpty(response.scrolls))
                {
                    print(response.scrolls);
                    ScrollInventoryContainer inventoryContainer = ScrollInventoryContainer.FromJson(response.scrolls);
                    inventoryContainer.SaveData();
                }

                if (!string.IsNullOrEmpty(response.summative))
                {
                    print(response.summative);
                    SummativeDataLog sumData = SummativeDataLog.FromJson(response.summative);
                    sumData.Save();
                }

                AlertManager.GetInstance().ShowInfoPanel("You have successfully logged in!", okeyAction: successLogin);
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

    public void ResendOtp()
    {
        string key = OtpKey.getKey();
        StartCoroutine(StartRequestResendOtp(key));
    }

    IEnumerator StartRequestResendOtp(string key)
    {
        ResendOtpData data = new ResendOtpData
        {
            key = key
        };

        string jsonData = JsonConvert.SerializeObject(data);
        string insertURL = LaravelRequest.GetLink("/api/user/resend-otp");

        using (UnityWebRequest www = new UnityWebRequest(insertURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                HandleResendErrorResponse(www);
            }
            else
            {
                HandleResendOtpSuccessResponse(www);
            }
        }

        refreshAlert();
    }

    void HandleResendErrorResponse(UnityWebRequest www)
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

                // Check if session expired
                if (errorMessage.Contains("expired") || errorMessage.Contains("Invalid Key"))
                {
                    otpPanel.SetActive(false);
                }

                AlertManager.GetInstance().ShowWarningPanel(errorMessage);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
                AlertManager.GetInstance().ShowErrorPanel("Error: " + www.error);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
            AlertManager.GetInstance().ShowErrorPanel("Server returned invalid data!");
        }
    }

    void HandleResendOtpSuccessResponse(UnityWebRequest www)
    {
        try
        {
            string jsonText = www.downloadHandler.text;
            Debug.Log("Server Response (Success): " + jsonText);

            logResponseData response = JsonConvert.DeserializeObject<logResponseData>(jsonText);

            if (response != null && response.status == "success")
            {
                OtpResponseData otpResponse = new OtpResponseData
                {
                    status = response.status,
                    message = response.message,
                    otp_key = response.otp_key
                };

                OtpKey.saveKey(otpResponse.otp_key);
                AlertManager.GetInstance().ShowInfoPanel("New OTP sent to your email!");
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
                Debug.LogError("Error: " + www.error);
                AlertManager.GetInstance().ShowErrorPanel("Error: " + www.error);
            }
        }
        catch (System.Exception e)
        {
            //Debug.LogError("JSON Parse Error: " + e.Message);
            AlertManager.GetInstance().ShowErrorPanel("Server returned invalid data!");
        }
    }

    public void successLogin()
    {
        SceneManager.LoadScene(2);
    }

    public void refreshAlert()
    {
        canvasAlertPanel.enabled = false;
        canvasAlertPanel.enabled = true;
    }
}

[System.Serializable]
public class logUserData
{
    public string email;
    public string password;

    public logUserData(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
}
public class MyData
{
    public string user_id;
    public string username;
    public string email;
    public string gender;
    public string grade_lvl;
    public string section_name;
    public bool isBanned; // Changed from string to bool

    // Saves all data to PlayerPrefs
    public void SaveAll()
    {
        if (!string.IsNullOrEmpty(user_id))
            PlayerPrefs.SetString("user_id", user_id);

        if (!string.IsNullOrEmpty(username))
            PlayerPrefs.SetString("username", username);

        if (!string.IsNullOrEmpty(email))
            PlayerPrefs.SetString("email", email);

        if (!string.IsNullOrEmpty(gender))
            PlayerPrefs.SetString("gender", gender);

        if (!string.IsNullOrEmpty(grade_lvl))
            PlayerPrefs.SetString("grade_lvl", grade_lvl);

        if (!string.IsNullOrEmpty(section_name))
            PlayerPrefs.SetString("section_name", section_name);

        PlayerPrefs.SetInt("isBanned", isBanned ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Loads data from PlayerPrefs
    public static MyData Load()
    {
        MyData data = new MyData
        {
            user_id = PlayerPrefs.GetString("user_id", ""),
            username = PlayerPrefs.GetString("username", ""),
            email = PlayerPrefs.GetString("email", ""),
            gender = PlayerPrefs.GetString("gender", ""),
            grade_lvl = PlayerPrefs.GetString("grade_lvl", ""),
            section_name = PlayerPrefs.GetString("section_name", ""),
            isBanned = PlayerPrefs.GetInt("isBanned", 0) == 1
        };
        return data;
    }

    // Alternative: Save/Load as JSON (better for complex data)
    public string ToJson() => JsonUtility.ToJson(this);
    public static MyData FromJson(string json) => JsonUtility.FromJson<MyData>(json);

    // Clears all saved user data
    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey("user_id");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("email");
        PlayerPrefs.DeleteKey("gender");
        PlayerPrefs.DeleteKey("grade_lvl");
        PlayerPrefs.DeleteKey("section_name");
        PlayerPrefs.DeleteKey("isBanned");
        PlayerPrefs.Save();

        Debug.Log("🧹 PlayerPrefs: All user data cleared.");
    }
}
