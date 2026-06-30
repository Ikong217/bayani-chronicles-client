using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ScrollShowManager : MonoBehaviour
{
    public static ScrollShowManager Instance;
    [Header("AudioClip")]
    public AudioClip typingClip;
    public AudioSource audioSource;

    [Header("Scroll Components")]
    public GameObject ParentContainer;
    //sub Scroll components
    public TextMeshProUGUI scrollTitle;
    public Image scrollImage;
    public TextMeshProUGUI imageTitle;
    public TextMeshProUGUI description;
    public Image largeImage;

    //back end
    private bool isScrollFinished = false;
    private string currentTextArea = "";

    private ScrollContent currentScroll;

    //private bool isScrollActive = false; //unused
    public float typingSpeed = 0.5f;
    public bool finished = false;
    public VoiceController voiceController;
    private bool isUsingVC = false;
    //private GameObject _player;
    private bool isFinishedReading;

    private TimerScript timer;

    private void Start()
    {
        //singleton instantiate
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (audioSource != null)
        {
            audioSource.clip = typingClip;
            audioSource.loop = true;
        }

        timer = TimerScript.Instance;
    }

    public void StartScroll(ScrollContent scrollContent)
    {
        isScrollFinished = true;
        //isScrollActive = true;
        ParentContainer.SetActive(true);
        isFinishedReading = false;
        currentScroll = scrollContent;
        finished = false;

        timer.StopTimer();

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (isFinishedReading)
        {
            EndScroll();
            return;
        }

        if (!isScrollFinished)
        {
            StopAllCoroutines();

            if (ParentContainer.activeSelf)
            {
                description.text = currentTextArea;
            }

            if (isUsingVC)
            {
                voiceController.StopSpeaking();
            }
            else
            {
                if (audioSource != null)
                    audioSource.Pause();
            }

            isScrollFinished = true;
            isFinishedReading = true;
            return;
        }

        isUsingVC = (PlayerPrefs.GetInt("playerTTSActive", 0) == 1);

        ParentContainer.SetActive(true);

        scrollTitle.text = currentScroll.title;
        scrollImage.sprite = currentScroll.sprite;
        largeImage.sprite = currentScroll.sprite;
        imageTitle.text = currentScroll.imageDescription;
        scrollTitle.text = currentScroll.title;

        currentTextArea = currentScroll.description;
        StartCoroutine(TypeSentences(currentTextArea));
    }

    IEnumerator TypeSentences(string textToType)
    {
        description.text = "";
        isScrollFinished = false;
        isFinishedReading = false;

        //print("isUsingVC: " + isUsingVC.ToString());
        if (isUsingVC)
        {
            //print("isUsingVC: " + isUsingVC.ToString());
            voiceController.StartSpeaking(textToType);
        }
        else
        {
            //print("nagplay Dito");
            if (audioSource != null)
                audioSource.Play();
        }

        foreach (char letter in textToType.ToCharArray())
        {
            description.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (!isUsingVC)
        {
            //print(isUsingVC);
            if (audioSource != null)
                audioSource.Pause();
        }

        isScrollFinished = true;
        isFinishedReading = true;
    }

    private void EndScroll()
    {
        timer.StartTimer();
        finished = true;
        ParentContainer.SetActive(false);
    }
}
