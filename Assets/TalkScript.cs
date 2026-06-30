using UnityEngine;
using UnityEngine.UI;

public class TalkScript : MonoBehaviour
{
    [SerializeField] private Button talkBtn;

    public Button GetTalkButton()
    {
        return talkBtn;
    }
}
