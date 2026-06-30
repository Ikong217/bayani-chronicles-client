
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ScrollInventoryDataScript : MonoBehaviour
{
    public bool clearDataOnStart = false;
    

    void Start()
    {
        ScrollInventoryContainer container = ScrollInventoryContainer.LoadData();
        Debug.Log("Loaded scrolls: " + container.scrollItem.Count);

        if (clearDataOnStart)
        {
            container.ClearData();
            Debug.Log("Scroll inventory cleared.");
        }
        print(this.gameObject);
    }
}

[System.Serializable]
public class ScrollInventoryContainer
{
    public List<ScrollInventoryItem> scrollItem = new List<ScrollInventoryItem>();
    public bool isSaved = false;

    public void SaveData()
    {
        if (scrollItem.Count <= 0)
        {
            Debug.LogWarning("No items to save.");
            return;
        }

        isSaved = true;
        PlayerPrefs.SetString("ScrollInventory", ToJson());
        PlayerPrefs.Save();
        Debug.Log("Inventory saved.");
    }

    public static int Count()
    {
        return LoadData().scrollItem.Count;
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteKey("ScrollInventory");
        scrollItem.Clear();
        isSaved = false;
    }

    public static ScrollInventoryContainer LoadData()
    {
        string json = PlayerPrefs.GetString("ScrollInventory", "");
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("No saved inventory found. Returning empty container.");
            return new ScrollInventoryContainer();
        }

        ScrollInventoryContainer container = FromJson(json);
        container.isSaved = true;
        return container;
    }

    public void AddScrollItem(string scrollName, string imageFilePath, string scrollTitle, Sprite scrollImage, string scrollImageTitle, string content)
    {
        if (FindScrollItem(scrollName) != null)
        {
            Debug.Log("Scroll already exists. Skipping add.");
            return;
        }

        scrollItem.Add(new ScrollInventoryItem(scrollName, imageFilePath, scrollTitle, scrollImage, scrollImageTitle, content));
        isSaved = false;
    }

    public ScrollInventoryItem FindScrollItem(string scrollName)
    {
        return scrollItem.Find(item => item.IsScroll(scrollName));
    }

    public string ToJson() => JsonUtility.ToJson(this);
    public static ScrollInventoryContainer FromJson(string json) => JsonUtility.FromJson<ScrollInventoryContainer>(json);
}

[System.Serializable]
public class ScrollInventoryItem
{
    [SerializeField] private string scrollName;
    [SerializeField] private string scrollTitle;
    [SerializeField] private string scrollImageName; // runtime-safe string
    [SerializeField] private string scrollImageTitle;
    [SerializeField] private string content;
    [SerializeField] private string imageFilePath;

    private Sprite scrollImage;

    public ScrollInventoryItem(string scrollName, string imageFilePath, string scrollTitle, Sprite scrollImage, string scrollImageTitle, string content)
    {
        if (string.IsNullOrEmpty(scrollName) || scrollImage == null || string.IsNullOrEmpty(content))
            return;

        this.scrollName = scrollName;
        this.scrollImage = scrollImage;
        //Debug.Log(scrollImage.ToString());

        //string ext = EnumHelper.GetImgType(imageType);
        string[] arr = scrollImage.name.Split('_');

        this.scrollTitle = scrollTitle;
        this.scrollImageName = arr[0];
        this.imageFilePath = imageFilePath;
        this.scrollImageTitle = scrollImageTitle;

        this.content = content;
    }

    public bool IsScroll(string name) => this.scrollName == name;

    public string GetContent() => content;
    public string GetScrollObjectName() => scrollName;
    public string GetScrollTitle() => scrollTitle;
    public string GetScrollImageTitle => scrollImageTitle;

    public Sprite GetSprite()
    {
        if (scrollImage == null && !string.IsNullOrEmpty(scrollImageName))
        {
            scrollImage = Resources.Load<Sprite>(imageFilePath + scrollImageName);

            if (scrollImage == null)
                Debug.LogWarning($"Sprite not found: {imageFilePath + scrollImageName}");
        }
        return scrollImage;
    }
}
