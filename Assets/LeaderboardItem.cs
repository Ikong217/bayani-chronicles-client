using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardItem : MonoBehaviour
{
    // Player data
    private string ranking;
    private string username;
    private string section;
    private string stars;
    private string totalScore;
    private string average;
    private string attempts;
    private Color color = Color.white;

    // UI elements
    private TextMeshProUGUI tmRanking;
    private TextMeshProUGUI tmUsername;
    private TextMeshProUGUI tmSection;
    private TextMeshProUGUI tmStars;
    private TextMeshProUGUI tmTotalScore;
    private TextMeshProUGUI tmAverage;
    private TextMeshProUGUI tmAttempts;
    private Image background;

    // Called automatically by Unity
    private void Awake()
    {
        GetAllTmps();
        background = GetComponent<Image>();
    }

    /// <summary>
    /// Initializes the leaderboard item with player data.
    /// </summary>
    public void Init(
        string ranking = "No Rank",
        string username = "No Username",
        string section = "No Section",
        string stars = "0",
        string totalScore = "00/00",
        string average = "00/10",
        string attempts = "0",
        Color? color = null)
    {
        this.ranking = ranking;
        this.username = username;
        this.section = section;
        this.stars = stars;
        this.totalScore = totalScore;
        this.average = average;
        this.attempts = attempts;
        this.color = color ?? Color.white;

        SetupTexts();
    }

    /// <summary>
    /// Assigns text values to the UI.
    /// </summary>
    private void SetupTexts()
    {
        if (background != null)
            background.color = color;

        if (tmRanking) tmRanking.text = ranking;
        if (tmUsername) tmUsername.text = username;
        if (tmSection) tmSection.text = section;
        if (tmStars) tmStars.text = stars;
        if (tmTotalScore) tmTotalScore.text = "Total Score: " + totalScore;
        if (tmAverage) tmAverage.text = "Average: " + average;
        if (tmAttempts) tmAttempts.text = "Attempts: " + attempts;
    }

    /// <summary>
    /// Finds and caches all TextMeshProUGUI components in the prefab hierarchy.
    /// </summary>
    private void GetAllTmps()
    {
        // Ranking
        Transform rankingParent = transform.Find("Ranking");
        if (rankingParent)
            tmRanking = rankingParent.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();

        // User profile
        Transform profileParent = transform.Find("User Profile");
        if (profileParent)
        {
            tmUsername = profileParent.Find("Username")?.GetComponent<TextMeshProUGUI>();
            tmSection = profileParent.Find("Username (1)")?.GetComponent<TextMeshProUGUI>();
        }

        // Overall credit
        Transform overallParent = transform.Find("Overall credit");
        if (overallParent)
        {
            tmStars = overallParent.Find("stars/star text")?.GetComponent<TextMeshProUGUI>();
            tmTotalScore = overallParent.Find("Total Scores")?.GetComponent<TextMeshProUGUI>();
        }

        // Additional credit
        Transform additionalParent = transform.Find("Additional Credit");
        if (additionalParent)
        {
            tmAverage = additionalParent.Find("average")?.GetComponent<TextMeshProUGUI>();
            tmAttempts = additionalParent.Find("attempts")?.GetComponent<TextMeshProUGUI>();
        }
    }
}
