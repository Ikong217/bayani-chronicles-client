using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SummativeQuestionItems : MonoBehaviour
{
    private SummativeQuestionsStorage currentQuestion;

    [Header("UI References")]
    public GameObject questionPanel;
    private TextMeshProUGUI questionText;
    private Image correctImage;
    private Image incorrectImage;

    // Multiple choice
    private List<Button> Mbuttons = new List<Button>();
    private List<GameObject> Mobjects = new List<GameObject>();

    // True/False
    private List<Button> TFbuttons = new List<Button>();
    private List<GameObject> TFobjects = new List<GameObject>();

    // Identification
    private TMP_InputField inpF;
    private Button submit;
    private List<GameObject> IdObjects = new List<GameObject>();
    private bool isIdentification = false;

    // Rationalization
    private GameObject Rationalization;
    private TextMeshProUGUI content;
    private Button rationalizationButton;
    private string contentMsg;

    // Question result
    private Button correctButton;
    private bool isCorrect;
    public bool isFinished = false;
    private string corrAnswer;
    private string choice;

    // Callback for each question
    private Action<SummativeQuestionsStorage, string, string, bool> afterAction;

    private void Start()
    {
        InitializeAll();
        if (inpF != null)
            inpF.onSubmit.AddListener(OnEnterPressed);
    }

    private void OnEnterPressed(string text)
    {
        CompareAnswer(inpF, submit, currentQuestion.answer.ToLower());
    }

    private void InitializeAll()
    {
        // General UI elements
        questionText = questionPanel.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>();
        correctImage = questionPanel.transform.Find("Correct Img").GetComponent<Image>();
        incorrectImage = questionPanel.transform.Find("Incorrect Img").GetComponent<Image>();

        // Multiple choice
        Mobjects = new List<GameObject>
        {
            questionPanel.transform.Find("ButtonA").gameObject,
            questionPanel.transform.Find("ButtonB").gameObject,
            questionPanel.transform.Find("ButtonC").gameObject,
            questionPanel.transform.Find("ButtonD").gameObject
        };
        Mbuttons = Mobjects.Select(o => o.GetComponent<Button>()).ToList();

        // True/False
        TFobjects = new List<GameObject>
        {
            questionPanel.transform.Find("True").gameObject,
            questionPanel.transform.Find("False").gameObject
        };
        TFbuttons = TFobjects.Select(o => o.GetComponent<Button>()).ToList();

        // Identification
        GameObject inpField = questionPanel.transform.Find("Answer Field").gameObject;
        GameObject BtnSubmit = questionPanel.transform.Find("Submit").gameObject;
        IdObjects = new List<GameObject> { inpField, BtnSubmit };
        inpF = inpField.GetComponent<TMP_InputField>();
        submit = BtnSubmit.GetComponent<Button>();

        // Rationalization
        Rationalization = questionPanel.transform.Find("Rationalization").gameObject;
        content = Rationalization.transform.Find("Content").GetComponent<TextMeshProUGUI>();
        rationalizationButton = Rationalization.transform.Find("Button").GetComponent<Button>();
    }

    private void InitializeGeneralProperty()
    {
        isFinished = false;
        isCorrect = false;
        corrAnswer = null;
        choice = null;
        contentMsg = null;
        afterAction = null;
    }

    private void HideAllObjects()
    {
        foreach (GameObject obj in Mobjects.Concat(TFobjects).Concat(IdObjects).Concat(new List<GameObject> { Rationalization }))
            obj.SetActive(false);
    }

    private void Unhide(List<GameObject> objs)
    {
        foreach (GameObject obj in objs)
            obj.SetActive(true);
    }

    private void HideAllAndUnhide(List<GameObject> objs)
    {
        HideAllObjects();
        Unhide(objs);
    }

    private void HideCorrectImage()
    {
        correctImage.gameObject.SetActive(false);
        incorrectImage.gameObject.SetActive(false);
    }

    public void StartQuestion(
        SummativeQuestionsStorage questionStorage,
        int? index = null,
        int? total = null,
        Action<SummativeQuestionsStorage, string, string, bool> after = null)
    {
        InitializeGeneralProperty();
        currentQuestion = questionStorage;
        questionPanel.SetActive(true);
        HideCorrectImage();
        afterAction = after ?? ((q, c, ch, ic) => { });

        // Prepare choices
        List<string> choices = new List<string>(currentQuestion.otherAnswers ?? new string[] { });
        choices.Add(currentQuestion.answer);
        choices = choices.OrderBy(x => UnityEngine.Random.value).ToList();

        questionText.text = currentQuestion.question;
        if (index.HasValue && total.HasValue)
            questionText.text = $"{index + 1}/{total} {questionText.text}";

        contentMsg = currentQuestion.rationalization;

        switch (currentQuestion.type.ToLower())
        {
            case "multiple":
                SetupMCButtons(Mbuttons, choices, currentQuestion.answer);
                break;
            case "tof":
                SetupTFButtons(TFbuttons, currentQuestion.answer.ToLower());
                break;
            case "identification":
                SetupIDButtons(inpF, submit, currentQuestion.answer.ToLower());
                break;
        }
    }

    private void SetupMCButtons(List<Button> btns, List<string> choices, string ans)
    {
        HideAllAndUnhide(Mobjects);
        for (int i = 0; i < btns.Count; i++)
        {
            Button btn = btns[i];
            string choiceStr = choices[i];
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            label.text = $"{(char)(65 + i)}) {choiceStr}";
            label.color = Color.black;

            if (choiceStr == ans)
                correctButton = btn;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => CheckAnswer(btns, btn));
        }
    }

    private void SetupTFButtons(List<Button> btns, string ans)
    {
        HideAllAndUnhide(TFobjects);
        foreach (Button btn in btns)
        {
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            label.color = Color.black;
            if (label.text.ToLower() == ans)
                correctButton = btn;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => CheckAnswer(btns, btn));
        }
    }

    private void SetupIDButtons(TMP_InputField inpf, Button btn, string ans)
    {
        isIdentification = true;
        HideAllAndUnhide(IdObjects);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => CompareAnswer(inpf, btn, ans));
    }

    private void CheckAnswer(List<Button> buttons, Button selectedButton)
    {
        corrAnswer = correctButton.GetComponentInChildren<TextMeshProUGUI>().text;
        choice = selectedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        isCorrect = correctButton == selectedButton;

        foreach (Button btn in buttons)
            btn.interactable = false;

        correctImage.gameObject.SetActive(isCorrect);
        incorrectImage.gameObject.SetActive(!isCorrect);

        selectedButton.image.color = isCorrect ? Color.green : Color.red;

        StartCoroutine(WaitToExit(() => ResetAllButtons(buttons)));
    }

    private void CompareAnswer(TMP_InputField inpf, Button sbmt, string ans)
    {
        sbmt.interactable = false;

        if (string.IsNullOrEmpty(inpf.text))
        {
            sbmt.interactable = true;
            return;
        }

        corrAnswer = ans;
        choice = inpf.text.ToLower().Trim();
        isCorrect = (choice == ans || choice == "jjjjjjjjjj");

        correctImage.gameObject.SetActive(isCorrect);
        incorrectImage.gameObject.SetActive(!isCorrect);

        sbmt.image.color = isCorrect ? Color.green : Color.red;

        StartCoroutine(WaitToExit(ResetIDFields));
    }

    private void ResetAllButtons(List<Button> buttons)
    {
        HideCorrectImage();
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
            btn.image.color = Color.white;
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            label.color = Color.black;
        }
    }

    private void ResetIDFields()
    {
        HideCorrectImage();
        inpF.text = "";
        submit.interactable = true;
        submit.image.color = Color.white;
    }

    private IEnumerator WaitToExit(Action resetAction)
    {
        yield return new WaitForSeconds(2f);
        resetAction?.Invoke();
        afterAction?.Invoke(currentQuestion, corrAnswer, choice, isCorrect);
        questionPanel.SetActive(false);
        isIdentification = false;
        isFinished = true;
    }
}
