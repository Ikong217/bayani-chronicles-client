using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompletePanelScript : MonoBehaviour
{
    private int questions = 0;
    private int score = 0;
    private int stars = 1;
    private Animator anim;
    public QuestionsRequestHandler questionHandler;
    public Button nextButton;
    [SerializeField] private Text scoreTxt;

    private void OnEnable()
    {
        questions = PlayerPrefs.GetInt("LevelQuestionCount", 0);
        score = PlayerPrefs.GetInt("PlayerScore", 0);
        AudioSource source = gameObject.GetComponent<AudioSource>();
        source.Play();

        if (score >= questions)
            stars = 3;
        else if (score >= questions / 2)
            stars = 2;
        else
            stars = 1;

        nextButton.enabled = false;

        anim = gameObject.GetComponent<Animator>();
        StartCoroutine(StartSuccessPanel());
    }

    IEnumerator StartSuccessPanel()
    {
        yield return null; // skips a frame
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        anim.SetInteger("star", stars);
        StartCoroutine(AnimateScores());

        string currentScene = SceneManager.GetActiveScene().name;
        Novels currentNovel = questionHandler.novel;

        PlayerNovels currentLevel = PlayerLevelsManager.Load(currentNovel);

        currentLevel.UpdateStars(currentScene, stars);
        if(currentLevel.UnlockNextLevel(currentScene) != null)
        {
            //print(PlayerLevelsManager.Load(currentNovel).ToJson());
            nextButton.enabled = true;
        }
    }

    IEnumerator AnimateScores()
    {
        scoreTxt.text = "0/" + questions.ToString();

        // Wait a moment before starting the animation (using unscaled time)
        float waitStartTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < waitStartTime + 0.5f)
        {
            yield return null;
        }

        float duration = 0.5f * stars; // Animation duration in seconds
        float timer = 0f;
        int currentScore = 0;

        while (timer < duration)
        {
            // Use unscaled time for the animation
            timer += Time.unscaledDeltaTime;
            float progress = timer / duration;
            currentScore = Mathf.FloorToInt(Mathf.Lerp(0, score, progress));
            scoreTxt.text = currentScore.ToString() + "/" + questions.ToString();
            yield return null;
        }

        // Ensure final score is displayed
        scoreTxt.text = score.ToString() + "/" + questions.ToString();
    }

    public void Restart()
    {
        SettingStatic.restart.Invoke();
    }

    public void Menu()
    {
        SettingStatic.menu.Invoke();
    }
    
    public void NextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Novels currentNovel = questionHandler.novel;

        PlayerNovels currentLevel = PlayerLevelsManager.Load(currentNovel);
        PlayerLevels nextLvl = currentLevel.FindNextLevel(currentScene);

        Time.timeScale = 1;
        SceneManager.LoadScene(nextLvl.Levelname);
    }
}