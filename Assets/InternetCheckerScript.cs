using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class InternetCheckerScript : MonoBehaviour
{
    [Header("Assign your panel (with Retry & Quit buttons)")]
    public GameObject internetPanel;

    [Tooltip("How often to check internet (seconds)")]
    public float checkInterval = 5f;

    private void Start()
    {
        if (internetPanel != null)
            internetPanel.SetActive(false);

        // Start checking connection every few seconds
        InvokeRepeating(nameof(CheckConnection), 0f, checkInterval);
    }

    private void CheckConnection()
    {
        if (!HasInternetConnection())
        {
            Time.timeScale = 0;
            if (internetPanel != null)
                internetPanel.SetActive(true);
        }
    }

    public void RetryReconnect()
    {
        if (HasInternetConnection())
        {
            Time.timeScale = 1;
            if (internetPanel != null)
                internetPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Still no internet connection.");
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stop play mode in editor
#else
        Application.Quit();
#endif
    }

    private bool HasInternetConnection()
    {
        try
        {
            using (var client = new WebClient())
            using (client.OpenRead("http://clients3.google.com/generate_204"))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
