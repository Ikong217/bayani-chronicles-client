using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class TimerScript : MonoBehaviour
{
    public static TimerScript Instance;
    private TextMeshProUGUI timeText;
    private float remainingTime = 0f;
    private bool isTimerRunning = false;
    private Animator anim;
    private float initialTime = 0f;
    private int lastMinute = -1;
    private int lastSecond = -1;
    private Coroutine timeOutCoroutine;

    private bool isLerping = false;
    private float lerpStartTime;
    private float lerpStartValue;
    private float lerpEndValue;
    private float lerpDuration = 0.5f; // smooth duration in seconds
    private GameObject deductTxt;
    [SerializeField] private GameObject DamageBG;
    [SerializeField] private GameObject QuestionPanel;

    [Header("Failed Panel in Canvas")]
    public GameObject failedPanel;

    [Header("Time Set")]
    [Min(0f)] public float minute = 0f;
    [Min(0f)] public float seconds = 0f;

    [Header("WarningStartTime")]
    [Min(0f)] public float warnMinute = 0f;
    [Min(0f)] public float warnSeconds = 0f;

    [Header("Activate if Infinite time")]
    [SerializeField] private bool isInfinite = false;

    private Action timeOutAction;
    private AudioSource timerAudio;
    private bool changed = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) { Destroy(gameObject); return; }

        timeText = GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        deductTxt = Resources.Load<GameObject>("Timer Deduct");
        timerAudio = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        timeOutAction = null;
        if(!changed)
            SetTimer(minute, seconds);
        StartTimer();
    }

    public void SetTimer(float minute, float second, Action outAction = null, float warnMinute = 0f, float warnSecond = 0f)
    {
        print($"timer set into {minute}:{second}");
        if (!isTimerRunning) isTimerRunning = true;
        timerAudio.Play();
        remainingTime = Mathf.Max(0f, (minute * 60f) + second);
        initialTime = remainingTime;
        timeOutAction = outAction;
        if (warnMinute > 0) this.warnMinute = warnMinute;
        if (warnSecond > 0) warnSeconds = warnSecond;
        changed = true;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (isLerping)
        {
            float t = (Time.time - lerpStartTime) / lerpDuration;
            remainingTime = Mathf.Lerp(lerpStartValue, lerpEndValue, t);

            if (t >= 1f)
            {
                remainingTime = lerpEndValue;
                isLerping = false;
            }

            UpdateTimerDisplay();
            return;
        }

        if (isTimerRunning && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (remainingTime <= 0)
            {
                print("Out OF time");
                remainingTime = 0;
                isTimerRunning = false;
                RunTimeOut();
            }
        }
    }

    public void DeductTimer(float time)
    {
        if (time <= 0f) return;

        // Start Lerp
        lerpStartValue = remainingTime;
        lerpEndValue = Mathf.Max(0f, remainingTime - time);
        lerpStartTime = Time.time;
        isLerping = true;

        // Instantiate the deduct text prefab
        if (deductTxt != null && timeText != null)
        {
            GameObject txtObj = Instantiate(deductTxt, timeText.transform.parent);
            TextMeshProUGUI txt = txtObj.GetComponent<TextMeshProUGUI>();
            if (txt != null)
            {
                int m = Mathf.FloorToInt(time / 60);
                int s = Mathf.FloorToInt(time % 60);
                txt.text = $"-{m:00}:{s:00}";
            }

            // Spawn slightly below the timer’s text
            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchoredPosition = timeText.rectTransform.anchoredPosition + new Vector2(0f, -10f);

            StartCoroutine(FallAndFadeDeductText(txtObj, 1.2f)); // duration ~1.2 sec
        }
    }


    IEnumerator FallAndFadeDeductText(GameObject txtObj, float duration)
    {
        DamageBG.SetActive(true);
        TextMeshProUGUI txt = txtObj.GetComponent<TextMeshProUGUI>();
        if (txt == null) yield break;

        Color originalColor = txt.color;
        Vector3 startPos = txt.rectTransform.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0f, -60f, 0f); // goes downwards

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // unaffected by pause
            float t = Mathf.Clamp01(elapsed / duration);

            // Gravity-like ease: starts fast, slows near end
            float ease = t * t;

            // Move down and fade out
            txt.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, ease);
            txt.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - ease);

            yield return null;
        }

        if (isOutOfTime()) RunTimeOut();

        DamageBG.SetActive(false);
        Destroy(txtObj);
    }

    private void UpdateTimerDisplay()
    {
        int timerMinute = Mathf.FloorToInt(remainingTime / 60);
        int timerSecond = Mathf.FloorToInt(remainingTime % 60);

        float warningTime = (warnMinute * 60) + warnSeconds;

        if (remainingTime <= 0 && isTimerRunning)
        {
            StopTimer();
            timeText.text = "00:00";
            return;
        }

        if (remainingTime <= warningTime)
        {
            timeText.color = Color.red;
            if (anim != null && (timerMinute != lastMinute || timerSecond != lastSecond))
                anim.SetTrigger("Warning");
        }
        else
        {
            timeText.color = Color.white;
            if (anim != null)
                anim.ResetTrigger("Warning");
        }

        lastMinute = timerMinute;
        lastSecond = timerSecond;
        timeText.text = $"{timerMinute:00}:{timerSecond:00}";
    }

    public void RunTimeOut()
    {
        if (isInfinite)
        {
            isTimerRunning = true;
            SetTimer(5f, 0f);
            return;
        }

        if (timeOutAction != null)
        {
            isTimerRunning = true;
            timeOutAction.Invoke();
            return;
        }

        if (QuestionPanel.activeSelf) QuestionPanel.SetActive(false);

        Debug.Log("Time's up!");
        timeText.text = "00:00";
        if (timeOutCoroutine != null) return;

        Time.timeScale = 0f;
        ScoreRequestHandler scoreRequest = ScoreRequestHandler.Instance;

        if (scoreRequest == null)
        {
            Debug.LogError("ScoreRequestHandler instance not found!");
            ActivateFailedPanel();
            return;
        }

        scoreRequest.StartRequestingScoreUpdate(GameLevelPlayedStatus.Failed, PlayerPrefs.GetInt("PlayerScore", 0));
        timeOutCoroutine = StartCoroutine(WaitSuccessRequest(scoreRequest));
    }

    IEnumerator WaitSuccessRequest(ScoreRequestHandler requestHandler)
    {
        if (requestHandler == null) { ActivateFailedPanel(); yield break; }

        yield return new WaitUntil(() => !requestHandler.externalRequest);
        yield return new WaitForSecondsRealtime(0.1f);

        ActivateFailedPanel();
        timeOutCoroutine = null;
    }

    private void ActivateFailedPanel()
    {
        PlayerPrefs.DeleteKey("PlayerScore");
        failedPanel.SetActive(true);
        AudioSource source = failedPanel.GetComponent<AudioSource>();
        source.Play();
        Animator anim = failedPanel.GetComponent<Animator>();
        StartCoroutine(AllowAnimateOnPause(anim));
    }

    IEnumerator AllowAnimateOnPause(Animator anim)
    {
        yield return null;
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        timerAudio.Play();
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        timerAudio.Pause();
    }
    public void ResetTimer()
    {
        remainingTime = initialTime;
        UpdateTimerDisplay();
        isTimerRunning = false;
    }
    public void ToggleTimer() => isTimerRunning = !isTimerRunning;
    public bool IsTimerRunning() => isTimerRunning;
    public bool IsTimerComplete() => remainingTime <= 0f;
    public float GetRemainingTime() => remainingTime;

    public void Restart() => SettingStatic.restart.Invoke();
    public void Menu() => SettingStatic.menu.Invoke();
    public bool isOutOfTime() => remainingTime <= 0;
}
