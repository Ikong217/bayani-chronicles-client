using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// Internet Speed Checker Class (Fixing HTTP/HTTPS Issues)
public class InternetSpeedChecker : MonoBehaviour
{
    public float downloadSpeed = 1f; // Default value

    public IEnumerator CheckInternetSpeed()
    {
        string url = "https://www.google.com"; // Use HTTPS instead of HTTP
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            downloadSpeed = Random.Range(5f, 50f); // Simulated speed
        }
        else
        {
            Debug.LogError("Internet speed check failed: " + request.error);
            downloadSpeed = 1f; // Fallback speed
        }
    }
}
