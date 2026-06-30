using UnityEngine;
using UnityEngine.UI;

public class TnCSctipt : MonoBehaviour
{
    [SerializeField] private Toggle tncToggle;
    [SerializeField] private GameObject tncPanel;
    private void Start()
    {
        tncToggle.isOn = false;
        tncToggle.enabled = false;
    }

    public void TnCPressed()
    {
        tncToggle.enabled = true;
        tncPanel.SetActive(true);
        tncToggle.isOn = true;
    }
}
