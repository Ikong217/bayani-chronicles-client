using UnityEngine;
using TMPro;

public class TTSTrialScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private VoiceController voiceController;
    public void OnMousePointerClick()
    {
        print(dialogue.text);
            voiceController.StartSpeaking(dialogue.text);
    }
}
