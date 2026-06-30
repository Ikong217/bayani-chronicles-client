using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Transform verticalLayout;
    [SerializeField] private GameObject sourceContent;

    private SummativeQuestionData qData;

    private void OnEnable()
    {
        // Load saved data safely
        SummativeDataLog data = SummativeDataLog.Load();

        if (data == null || data.logs == null || data.logs.Count == 0)
        {
            Debug.LogWarning("No summative data found.");
            return;
        }

        qData = data.logs[^1];

        score.text = $"{qData.score}/{qData.summativeQuestionContainer.questions.Count}";

        // Clear previous UI
        foreach (Transform child in verticalLayout)
        {
            Destroy(child.gameObject);
        }

        // Load prefab once
        if (sourceContent == null)
        {
            sourceContent = Resources.Load<GameObject>("Content");
            if (sourceContent == null)
            {
                Debug.LogError("Content prefab not found in Resources!");
                return;
            }
        }

        // Populate results
        foreach (SummativeQuestionsStorage item in qData.summativeQuestionContainer.questions)
        {
            GameObject tally = Instantiate(sourceContent, verticalLayout);

            tally.transform.Find("Question")
                .GetComponent<TextMeshProUGUI>().text = $"Q: {item.question}";

            tally.transform.Find("Answer")
                .GetComponent<TextMeshProUGUI>().text = $"Correct Answer: {item.correctAnswer}";

            tally.transform.Find("Choice")
                .GetComponent<TextMeshProUGUI>().text = $"Your Answer: {item.choice}";

            tally.GetComponent<Image>().color = item.isCorrect ? Color.green : Color.red;
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainScene");
    }
}
