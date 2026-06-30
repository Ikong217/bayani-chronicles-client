using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordToggleEye : MonoBehaviour
{
    private Sprite openEye;
    private Sprite closedEye;

    private TMP_InputField passwordInp;
    private Button eyeBtn;

    [SerializeField] bool eyeOpen = false; // false = hidden (Password), true = visible (Standard)

    private void Start()
    {
        // Load your sprites from Resources folder (Assets/Resources/)
        openEye = Resources.Load<Sprite>("eye(1)");     // <-- make sure file names match!
        closedEye = Resources.Load<Sprite>("eyebrow"); // <-- adjust to your actual file names

        passwordInp = GetComponent<TMP_InputField>();
        eyeBtn = transform.Find("Eye").GetComponent<Button>();

        eyeBtn.onClick.AddListener(ToggleEye);
        //eyeBtn.image.sprite = eyeOpen ? openEye : closedEye;

        eyeOpen = !eyeOpen;
        ToggleEye();
    }

    private void ToggleEye()
    {
        eyeOpen = !eyeOpen;

        // Change icon
        eyeBtn.image.sprite = eyeOpen ? openEye : closedEye;

        // Switch visibility mode
        passwordInp.contentType = eyeOpen ?
            TMP_InputField.ContentType.Password :
            TMP_InputField.ContentType.Standard;

        // Refresh display
        passwordInp.ForceLabelUpdate();
    }
}
