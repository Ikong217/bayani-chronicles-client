using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; // Added for Action type

public class LevelSettingsScript : MonoBehaviour
{
    private bool ispause = false;
    public GameObject settingsWindow;
    private Animator settingsAnimator;
    private Coroutine restartCoroutine;
    private Coroutine exitCoroutine;
    public MyConfirmationScript confirmationScript;
    public Toggle settingsTTSButton;
    //private bool isTTSOn = false;
    public Toggle settingsJsFixedButton;
    //private bool isJsFxOn = false;

    private void Start()
    {
        if (settingsWindow == null)
            Debug.LogError("Missing component: Settings Window");

        settingsAnimator = settingsWindow.GetComponent<Animator>();
        if (settingsAnimator == null)
        {
            Debug.LogError("Missing component: Settings Animator");
        }

        if (confirmationScript == null)
        {
            Debug.LogError("Missing component: Confirmation Script");
        }

        if(!PlayerPrefs.HasKey("playerTTSActive"))
        {
            print("it does not have a key");
            PlayerPrefs.SetInt("playerTTSActive", 1);
        }
        settingsTTSButton.isOn = (PlayerPrefs.GetInt("playerTTSActive", 0) == 1);
        settingsJsFixedButton.isOn = (PlayerPrefs.GetInt("playerJsActive", 0) == 1);    
        //print(PlayerPrefs.GetInt("playerTTSActive", 0));

    }

    public void TTSToggle()
    {
        print(settingsTTSButton.isOn);
        PlayerPrefs.SetInt("playerTTSActive", settingsTTSButton.isOn ? 1 : 0);
    }
    public void JsFixedToggle()
    {
        print(settingsJsFixedButton.isOn);
        PlayerPrefs.SetInt("playerJsActive", settingsJsFixedButton.isOn ? 1 : 0);
    }

    public void TogglePause()
    {
        ispause = !ispause;
        if (ispause)
            Pause();
        else
            Resume();
    }

    public void Resume()
    {
        Debug.Log("Resume");
        CloseSettingsWindow();
    }

    public void Pause()
    {
        Debug.Log("Pause");
        ActivateSettingsWindow();
    }

    public void ClickExit()
    {
        if (confirmationScript == null)
        {
            Debug.LogError("Confirmation script not assigned!");
            return;
        }

        string message = "Are you sure you want to exit? You will lose your progress!!";
        confirmationScript.OpenConfirmation(message, Exit);
    }

    public void ClickRestart()
    {
        if (confirmationScript == null)
        {
            Debug.LogError("Confirmation script not assigned!");
            return;
        }

        string message = "Are you sure you want to Restart? You will reset your progress!!";
        confirmationScript.OpenConfirmation(message, Restart);
    }

    public void Exit()
    {
        Debug.Log("Exit pressed");
        if (exitCoroutine != null)
        {
            Debug.LogWarning("Exit already in progress");
            return;
        }

        Time.timeScale = 1f;
        ScoreRequestHandler scoreRequest = ScoreRequestHandler.Instance;

        if (scoreRequest == null)
        {
            Debug.LogError("ScoreRequestHandler instance not found!");
            ExitImmediate();
            return;
        }

        scoreRequest.StartRequestingScoreUpdate(GameLevelPlayedStatus.Quit, 0);
        exitCoroutine = StartCoroutine(WaitSuccessRequestExit(scoreRequest));
    }

    public void ActivateSettingsWindow()
    {
        if (settingsWindow == null) return;

        settingsWindow.SetActive(true);
        StartCoroutine(StartPopUpAnimate());
    }

    IEnumerator StartPopUpAnimate()
    {
        if (settingsAnimator == null) yield break;

        settingsAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        yield return new WaitForSecondsRealtime(0.01f); // Wait one frame

        settingsAnimator.SetTrigger("PopUp");

        // Wait for animation to complete (using unscaled time)
        yield return new WaitForSecondsRealtime(settingsAnimator.GetCurrentAnimatorStateInfo(0).length);

        Time.timeScale = 0;
        ispause = true;
    }

    public void CloseSettingsWindow()
    {
        Time.timeScale = 1f;
        ispause = false;

        StartCoroutine(StartPopOutAnimation());
    }

    IEnumerator StartPopOutAnimation()
    {
        if (settingsAnimator == null)
        {
            settingsWindow.SetActive(false);
            yield break;
        }

        settingsAnimator.SetTrigger("PopOut");
        yield return new WaitForSecondsRealtime(settingsAnimator.GetCurrentAnimatorStateInfo(0).length);

        settingsWindow.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Restart()
    {
        Debug.Log("Restart pressed");
        if (restartCoroutine != null)
        {
            Debug.LogWarning("Restart already in progress");
            return;
        }

        Time.timeScale = 1f;
        ScoreRequestHandler scoreRequest = ScoreRequestHandler.Instance;

        if (scoreRequest == null)
        {
            Debug.LogError("ScoreRequestHandler instance not found!");
            RestartImmediate();
            return;
        }

        scoreRequest.StartRequestingScoreUpdate(GameLevelPlayedStatus.Restart, 0);
        restartCoroutine = StartCoroutine(WaitSuccessRequest(scoreRequest));
    }

    IEnumerator WaitSuccessRequest(ScoreRequestHandler requestHandler)
    {
        if (requestHandler == null)
        {
            RestartImmediate();
            yield break;
        }

        yield return new WaitUntil(() => requestHandler.externalRequest == false);
        yield return new WaitForSecondsRealtime(0.1f);

        if (requestHandler.externalSuccess)
        {
            RestartImmediate();
        }
        else
        {
            Debug.LogWarning("Score update failed, retrying restart...");
            yield return new WaitForSecondsRealtime(1f);
            Restart();
        }

        restartCoroutine = null;
    }

    IEnumerator WaitSuccessRequestExit(ScoreRequestHandler requestHandler)
    {
        if (requestHandler == null)
        {
            ExitImmediate();
            yield break;
        }

        yield return new WaitUntil(() => requestHandler.externalRequest == false);
        yield return new WaitForSecondsRealtime(0.1f);

        if (requestHandler.externalSuccess)
        {
            ExitImmediate();
        }
        else
        {
            Debug.LogWarning("Exit request failed, trying again...");
            yield return new WaitForSecondsRealtime(1f);
            Exit();
        }

        restartCoroutine = null;
    }

    // Fixed method name - was ExitImmidiate (with 2 i's)
    public void ExitImmediate()
    {
        SceneManager.LoadScene("Scenes/MainScene");
    }

    public void RestartImmediate()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Time.timeScale = 1f;
    }
}

public static class SettingStatic
{
    public static Action restart = Restart;
    public static Action menu = Menu;
    public static void Restart()
    {
        // Reload the current active scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void Menu()
    {
        // Load the main menu scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }
}