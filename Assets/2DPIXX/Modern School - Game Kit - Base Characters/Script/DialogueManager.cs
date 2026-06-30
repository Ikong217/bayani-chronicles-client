using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public AudioClip typingClip;
    public AudioSource audioSource;

    public GameObject ParentDialogue;

    public GameObject dialogueBox;
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    public GameObject itemDialogueBox;
    public Image itemImage;
    public TextMeshProUGUI itemText;
    public Image largeImage;

    private bool isDialogueFinished = false;
    private string currentTextArea = "";

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();

    private bool isDialogueActive = false;
    public float typingSpeed = 0.5f;
    public bool finished = false;
    public VoiceController voiceController;
    private bool isUsingVC = false;
    private bool subEvent = false;
    private GameObject _player;
    private Camera cam;
    private CamFollow camscr;

    private MiscManager miscManager;

    public CamFollow GetCam() => camscr;

    public void Start()
    {
        if (Instance == null)
        { 
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //audioSource = gameObject.GetComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.clip = typingClip;
            audioSource.loop =  true;
        }

        miscManager = gameObject.GetComponent<MiscManager>();
        GameObject coreComponent = GameObject.Find("Core Level Components");
        cam = coreComponent.transform.Find("Main Camera").GetComponent<Camera>();
        camscr = cam.GetComponent<CamFollow>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //print("dialogue started");
        finished = false;
        lines.Clear();
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
        if(lines.Count < 1)
        {
            EndDialogue();
            return;
        }
        //print(lines.Count);
        isDialogueFinished = true;
        isDialogueActive = true;
        ParentDialogue.SetActive(true);

        if (miscManager.isRunning())
        {
            subEvent = true;
        }
        else
        {
            subEvent = false;
            miscManager.StartEvent();
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (!isDialogueFinished)
        {
            StopAllCoroutines();

            if (dialogueBox.activeSelf)
            {
                dialogueArea.text = currentTextArea; // Fill the remaining text immediately
            }
            else if (itemDialogueBox.activeSelf)
            {
                itemText.text = currentTextArea;
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

            isDialogueFinished = true;
            return;
        }

        dialogueBox.SetActive(false);
        itemDialogueBox.SetActive(false);

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentline = lines.Dequeue();

        if (currentline.dialogueType == DialogueType.character)
        {
            GameObject character = _player;
            Character classCharacter = null;

            isUsingVC = (PlayerPrefs.GetInt("playerTTSActive", 0) == 1);
            

            dialogueBox.SetActive(true);
            if (currentline.player)
            {
                character = Player.FindPlayer();
                MyPlayer myPlayer = character.GetComponent<MyPlayer>();
                classCharacter = myPlayer as Character;
            }
            else
            {
                character = currentline.characterObj;

                Teacher teacher = character.GetComponent<Teacher>();
                Student student = character.GetComponent<Student>();
                if (teacher != null)
                    classCharacter = teacher as Character;
                else if (student != null)
                    classCharacter = student as Character;
                else
                {
                    classCharacter = new Character();
                    SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                    
                    if (sprite != null)
                    {
                        classCharacter.SetSrite(sprite.sprite);
                        classCharacter.SetName("");
                    }
                }
                    
            }

            if (currentline.cameraFollow)
                camscr.SetTargetPosition(character.transform);

            characterIcon.sprite = classCharacter.GetSprite();
            characterName.text = classCharacter.GetName();

            string player = Player.FindPlayer().GetComponent<MyPlayer>().GetName();
            string me = classCharacter.GetName();
            LockMechanism lm = LockMechanism.GetInstance();
            //print(lm.GetPassword());
            string password = lm != null? lm.GetPassword() : "[no Password Available]";
            currentTextArea = currentline.line;
            System.DateTime DatetimeNow = System.DateTime.Now;
            string timeNowStr = DatetimeNow.TimeOfDay.ToString();
            string dateNow = DatetimeNow.Date.ToString();
            string meridian = DatetimeNow.Hour > 12 ? "Hapon" : "Umaga"; 

            currentTextArea = currentTextArea.Replace("{me}", me);
            currentTextArea = currentTextArea.Replace("{player}", player);
            currentTextArea = currentTextArea.Replace("{password}", password);
            currentTextArea = currentTextArea.Replace("{timenow}", timeNowStr);
            currentTextArea = currentTextArea.Replace("{datenow}", dateNow);
            currentTextArea = currentTextArea.Replace("{meridian}", meridian);

            StopAllCoroutines();
            StartCoroutine(TypeSentences(currentTextArea));
        }
        else if (currentline.dialogueType == DialogueType.item)
        {
            itemDialogueBox.SetActive(true);
            itemImage.sprite = currentline.sprite;
            largeImage.sprite = currentline.sprite;

            currentTextArea = currentline.line;

            LockMechanism lm = LockMechanism.GetInstance();
            //print(lm);
            string password = lm != null ? lm.GetPassword() : "[no Password Available]";
            currentTextArea = currentTextArea.Replace("{password}", password);

            StopAllCoroutines();
            StartCoroutine(ItemTypeSentences(currentTextArea));
        }
    }

    public void ToggleLargeImage()
    {
        if (largeImage != null)
        {
            largeImage.gameObject.SetActive(!largeImage.gameObject.activeSelf);
        }
    }

    IEnumerator TypeSentences(string textToType)
    {
        dialogueArea.text = "";
        isDialogueFinished = false;

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
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if(!isUsingVC)
        {
            //print(isUsingVC);
            if (audioSource != null)
                audioSource.Pause();
        }

        isDialogueFinished = true;
    }

    IEnumerator ItemTypeSentences(string textToType)
    {
        itemText.text = "";
        isDialogueFinished = false;

        if (isUsingVC)
        {
            voiceController.StartSpeaking(textToType);
        }
        else
        {
            //print("napunta Dito");
            if (audioSource != null)
                audioSource.Play();
        }

        foreach (char letter in textToType.ToCharArray())
        {
            itemText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (!isUsingVC)
        {
            if (audioSource != null)
                audioSource.Pause();
        }

        isDialogueFinished = true;
    }


    void EndDialogue()
    {
        if (subEvent)
        {
            subEvent = false;
        }
        else
        {
            miscManager.EndEvent();
        }

        isDialogueActive = false;
        if (ParentDialogue.activeSelf) // Only deactivate if it was previously active
        {
            ParentDialogue.SetActive(false);
        }
        finished = true;
    }


}
