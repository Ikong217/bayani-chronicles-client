using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollMenuScript : MonoBehaviour
{
    public Text titleTxt;
    public Image img;
    public TextMeshProUGUI desc;

    public void SetScrollMenu(string scrollName)
    {
        ScrollInventoryItem scrollItem = ScrollInventoryContainer.LoadData().FindScrollItem(scrollName);
        if (scrollItem == null)
        {
            Debug.LogWarning($"Scroll item not found: {scrollName}");
            return;
        }

        string[] parts = scrollItem.GetScrollObjectName().Split('/');
        string novel = parts.Length > 0 ? parts[0] : "Unknown Novel";
        string level = parts.Length > 1 ? parts[1] : "Unknown Level";

        titleTxt.text = novel + " - " + level;

        img.sprite = scrollItem.GetSprite(); // now runtime-safe
        desc.text = scrollItem.GetContent();
    }
}
