using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestionnaireItems : MonoBehaviour
{
    private QuestionsStorage currentQuestion;
    public GameObject questionPanel;

    private TextMeshProUGUI questionText;
    private Image correctImage;
    private Image incorrectImage;

    // Multiple choice buttons
    private List<Button> Mbuttons = new List<Button>();
    private List<GameObject> Mobjects = new List<GameObject>();

    // True or False buttons
    private List<Button> TFbuttons = new List<Button>();
    private List<GameObject> TFobjects = new List<GameObject>();

    // Identification
    private TMP_InputField inpF;
    private Button submit;
    private List<GameObject> IdObjects = new List<GameObject>();

    // Rationalization
    private GameObject Rationalization;
    private TextMeshProUGUI content;
    private string contentMsg;
    private Button rationalizationButton;

    private Button correctButton;
    private bool isCorrect;
    public bool isFinished = false;

    private Action corrAction;
    private Action incAction;

    //identification on enter next
    private bool isIdentification = false;

    private void Start()
    {
        InitializeAll();
        inpF.onSubmit.AddListener(onEnterPressed);
    }

    void onEnterPressed(string text)
    {
        CompareAnswer(inpF, submit, currentQuestion.answer.ToLower());
    }

    private void InitializeAll()
    {
        InitializeGeneralProperty();

        questionText = questionPanel.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>();
        correctImage = questionPanel.transform.Find("Correct Img").GetComponent<Image>();
        incorrectImage = questionPanel.transform.Find("Incorrect Img").GetComponent<Image>();

        // Multiple choice
        GameObject choiceA = questionPanel.transform.Find("ButtonA").gameObject;
        GameObject choiceB = questionPanel.transform.Find("ButtonB").gameObject;
        GameObject choiceC = questionPanel.transform.Find("ButtonC").gameObject;
        GameObject choiceD = questionPanel.transform.Find("ButtonD").gameObject;
        Mobjects = new List<GameObject> { choiceA, choiceB, choiceC, choiceD };
        Mbuttons = Mobjects.Select(obj => obj.GetComponent<Button>()).ToList();

        // True / False
        GameObject BtnTrue = questionPanel.transform.Find("True").gameObject;
        GameObject BtnFalse = questionPanel.transform.Find("False").gameObject;
        TFobjects = new List<GameObject> { BtnTrue, BtnFalse };
        TFbuttons = TFobjects.Select(obj => obj.GetComponent<Button>()).ToList();

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
        contentMsg = null;
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

    private void DisableAllObjects()
    {
        foreach (Button obj in Mbuttons.Concat(TFbuttons))
            obj.enabled = false;
        submit.enabled = false;
        inpF.enabled = false;
    }

    private void EnableAllObjects()
    {
        foreach (Button obj in Mbuttons.Concat(TFbuttons))
            obj.enabled = true;
        submit.enabled = true;
        inpF.enabled = true;
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

    private void SetupMCButtons(List<Button> btns, List<string> strs, string ans)
    {
        HideAllAndUnhide(Mobjects);

        for (int i = 0; i < btns.Count; i++)
        {
            Button btn = btns[i];
            string choice = strs[i];

            var label = btn.GetComponentInChildren<Text>();
            label.text = $"{(char)(65 + i)}) {choice}";
            label.color = Color.black;

            if (choice == ans)
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
            var label = btn.GetComponentInChildren<Text>();
            string btnStr = label.text.ToLower();
            label.color = Color.black;

            if (btnStr == ans)
                correctButton = btn;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => CheckAnswer(btns, btn));
        }
    }

    private void SetupIDButtons(TMP_InputField inpfd, Button btn, string ans)
    {
        isIdentification = true;
        HideAllAndUnhide(IdObjects);

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => CompareAnswer(inpfd, btn, ans));
    }

    public void StartQuestion(
        QuestionsStorage questionsStorage,
        int? index = null,
        int? total = null,
        Action correctAction = null,
        Action incorrectAction = null)
    {
        isIdentification = false;
        //print("Question Started");
        InitializeGeneralProperty();
        currentQuestion = questionsStorage;
        questionPanel.SetActive(true);
        HideCorrectImage();

        List<string> choices = new List<string>(currentQuestion.otherAnswers);
        choices.Add(currentQuestion.answer);
        choices = choices.OrderBy(x => UnityEngine.Random.value).ToList();

        questionText.text = currentQuestion.question;
        if (index.HasValue && total.HasValue)
        {
            questionText.text = $"{index + 1}/{total} {questionText.text}";
        }

        contentMsg = currentQuestion.rationalization;
        //print(currentQuestion.rationalization);

        corrAction = correctAction ?? (() => { });
        incAction = incorrectAction ?? (() => { });

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

    private void CheckAnswer(List<Button> buttons, Button selectedButton)
    {
        foreach (Button btn in buttons)
            btn.interactable = false;

        if (correctButton == selectedButton)
        {
            isCorrect = true;
            correctImage.gameObject.SetActive(true);
            selectedButton.image.color = Color.green;
        }
        else
        {
            isCorrect = false;
            incorrectImage.gameObject.SetActive(true);
            selectedButton.image.color = Color.red;
        }

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

        if (ans == inpf.text.ToLower().Trim() || inpf.text.ToLower().Trim() == "jjjjjjjjjj")
        {
            isCorrect = true;
            correctImage.gameObject.SetActive(true);
            sbmt.image.color = Color.green;
        }
        else
        {
            isCorrect = false;
            incorrectImage.gameObject.SetActive(true);
            sbmt.image.color = Color.red;
        }

        StartCoroutine(WaitToExit(ResetIDFields));
    }

    private void ResetAllButtons(List<Button> buttons)
    {
        correctImage.gameObject.SetActive(false);
        incorrectImage.gameObject.SetActive(false);

        foreach (Button btn in buttons)
        {
            btn.interactable = true;
            btn.image.color = Color.white;
            var label = btn.GetComponentInChildren<Text>();
            label.color = Color.black;
        }
    }

    private void ResetIDFields()
    {
        correctImage.gameObject.SetActive(false);
        incorrectImage.gameObject.SetActive(false);
        inpF.text = "";
        submit.image.color = Color.white;
        submit.interactable = true;
    }

    public void RunTimeOut()
    {
        DisableAllObjects();
        isCorrect = false;
        incorrectImage.gameObject.SetActive(true);

        StartCoroutine(WaitToExit(() => EnableAllObjects()));
    }

    private IEnumerator WaitToExit(Action resetAction)
    {
        yield return new WaitForSeconds(2f);
        resetAction.Invoke();

        // show the rationalization
        if (isCorrect)
            RenderRationalization(corrAction);
        else
            RenderRationalization(incAction);

        //if (isCorrect)
        //{

        // wait up to 10 seconds or until it's closed by button
        float timer = 0f;
        while (Rationalization.activeSelf && timer < 10f)
        {
            //print(timer);
            timer += Time.deltaTime;
            yield return null;
        }

        // close rationalization after 10s if not closed manually
        if (Rationalization.activeSelf)
            TermenateRatinalization(corrAction);
        //}
        //else
        //{
            //incAction.Invoke();
        //}
        questionPanel.SetActive(false);
        isIdentification = false;
        isFinished = true;
    }

    private void RenderRationalization(Action action)
    {
        Rationalization.SetActive(true);
        content.text = contentMsg ?? "No Given Rationalization in this Item";
        //print(content.text);

        // Stop timer while rationalization is open
        TimerScript.Instance.StopTimer();

        rationalizationButton.onClick.RemoveAllListeners();
        rationalizationButton.onClick.AddListener(() =>
        {
            // When user clicks the close button, immediately close
            TermenateRatinalization(action);
        });
    }

    private void TermenateRatinalization(Action action)
    {
        //print("terminated");
        if (!Rationalization.activeSelf) return; // prevent double-calls

        Rationalization.SetActive(false);
        TimerScript.Instance.StartTimer();
        action.Invoke();
    }

}
