using UnityEngine;
using UnityEngine.Android;

public class VoiceController : MonoBehaviour
{
    const string LANG_CODE = "fil-PH";

    private void Start()
    {
        SetUp(LANG_CODE);

#if UNITY_ANDROID
        //TextSpeech.SpeechToText.Instance.onPartialResultsCallback = ;
#endif

        TextSpeech.TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextSpeech.TextToSpeech.Instance.onDoneCallback = OnSpeakStop;
    }
    void SetUp(string code)
    {
        TextSpeech.TextToSpeech.Instance.Setting(code, 1, 1);
    }

    void CheckPermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
    }


    //Text to speech commands
    public void StartSpeaking(string message)
    {
        TextSpeech.TextToSpeech.Instance.StartSpeak(message);
        //print("Speaking:: " + message);
    }

    public void StopSpeaking()
    {
        TextSpeech.TextToSpeech.Instance.StopSpeak();
        //print("Speaking Stopped");
    }

    void OnSpeakStart()
    {
        print("Speaking Started...");
    }
    void OnSpeakStop()
    {
        print("Speaking Stopped...");
    }
}
