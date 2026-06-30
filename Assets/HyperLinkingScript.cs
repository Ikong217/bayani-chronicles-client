using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class HyperlinkObjects
{
    public GameObject obj = null;
    public string link = "";
    public bool projectWebsite = false;
}

[System.Serializable]
public class HyperlinkObjectsContainer
{
    public List<HyperlinkObjects> hyperlinkObjects = new List<HyperlinkObjects>();
}

public class HyperLinkingScript : MonoBehaviour
{
    public HyperlinkObjectsContainer hyperlinkObjects;

    private void Start()
    {
        foreach (HyperlinkObjects ho in hyperlinkObjects.hyperlinkObjects)
        {
            if (ho.obj == null) continue;

            string url = ho.projectWebsite ? LaravelRequest.GetLink(ho.link) : ho.link;

            // Ensure scheme
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            // Make sure it has a Button
            Button btn = ho.obj.GetComponent<Button>();
            if (btn == null)
                btn = ho.obj.AddComponent<Button>();

            // Clear old listeners just in case
            btn.onClick.RemoveAllListeners();

            // Add new one
            btn.onClick.AddListener(() => OnLinkClick(url));
        }
    }

    public void OnLinkClick(string url)
    {
        Debug.Log("Opening URL: " + url);
        Application.OpenURL(url);
    }
}
