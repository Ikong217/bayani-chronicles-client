using UnityEngine;
using TMPro;

public class Username : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();

        text.text = MyData.Load().username;
    }
}
