using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryMenuScript : MonoBehaviour
{
    public GameObject scrollButton;       // Prefab for the scroll button
    public AudioSource source;
    public GameObject scrollableLayout;   // Parent container for buttons
    public MainMenu mainMenu;
    public Sprite[] scrollImages;

    private void Start()
    {
        RemoveAllChild();
        RenderScrollData();
    }

    private void RemoveAllChild()
    {
        foreach (Transform child in scrollableLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void RenderScrollData()
    {
        ScrollInventoryContainer scrollContainer = ScrollInventoryContainer.LoadData();

        foreach (ScrollInventoryItem item in scrollContainer.scrollItem)
        {
            GameObject button = Instantiate(scrollButton, scrollableLayout.transform);

            // 🔹 make a local copy so each listener remembers its own item
            ScrollInventoryItem capturedItem = item;

            StartCoroutine(LoadVisual(button, capturedItem));
        }
    }

    IEnumerator LoadVisual(GameObject button, ScrollInventoryItem item)
    {
        //print("corouted");
        yield return null; // skips a frame to instantiate the object first

        // ✅ Button is on the prefab itself
        Button btn = button.GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("No Button component found on prefab: " + button.name);
            yield break;
        }
        btn.enabled = true;

        Image btnImg = button.GetComponent<Image>();
        if (btnImg != null) btnImg.enabled = true;

        //print(item.GetScrollObjectName());

        // Add click listeners
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            Debug.Log("Clicked: " + item.GetScrollObjectName());
            source.Play();
            mainMenu.ScrollMenuPressed(item.GetScrollObjectName());
        });

        // Set image safely
        Transform imageTransform = button.transform.Find("Image");
        if (imageTransform != null)
        {
            Image img = imageTransform.GetComponent<Image>();
            if (img != null && scrollImages.Length > 0)
            {
                int randIndex = Random.Range(0, scrollImages.Length);
                img.sprite = scrollImages[randIndex];
                img.enabled = true;
            }
        }
    }
}
