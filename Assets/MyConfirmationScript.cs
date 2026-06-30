using UnityEngine;
using System;
using TMPro;
using System.Collections;

public class MyConfirmationScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private Animator dialogAnimator;

    public Action OKFunction;
    private bool isActive = false;
    private bool isAnimating = false;

    private void Start()
    {
        dialogPanel = gameObject;
        dialogAnimator = gameObject.GetComponent<Animator>();
        //dialogPanel.SetActive(true);
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
        Reset();
    }

    public void OpenConfirmation(string message, Action okAction)
    {
        //print(dialogPanel);
        if (isActive || isAnimating) return;

        OKFunction = okAction;
        text.text = message;
        isAnimating = true;
        print("comes here");

        if (dialogPanel != null)
            dialogPanel.SetActive(true);

        // Start animation coroutine
        StartCoroutine(OpenAnimation());
    }

    private IEnumerator OpenAnimation()
    {
        print("napunta dito");
        // Wait for one frame para ma-render muna yung UI
        yield return null;

        if (dialogAnimator != null)
        {
            print("animator exist");
            dialogAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        if (dialogAnimator != null)
        {
            dialogAnimator.SetTrigger("Open");
            yield return new WaitForSecondsRealtime(dialogAnimator.GetCurrentAnimatorStateInfo(0).length);
        }

        isActive = true;
        isAnimating = false;
    }

    public void OK()
    {
        if (!isActive || isAnimating) return;

        StartCoroutine(CloseAnimationAndInvoke(true));
    }

    public void Cancel()
    {
        print("pressed cancel");
        if (!isActive || isAnimating) return;
        print("returnting;");

        StartCoroutine(CloseAnimationAndInvoke(false));
    }

    private IEnumerator CloseAnimationAndInvoke(bool confirm)
    {
        isAnimating = true;
        isActive = false;
        print("comes here");
        if (dialogAnimator != null)
        {
            dialogAnimator.SetTrigger("Close");
            yield return new WaitForSecondsRealtime(dialogAnimator.GetCurrentAnimatorStateInfo(0).length);
        }

        if (confirm)
        {
            OKFunction?.Invoke();
        }

        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        Reset();
        isAnimating = false;
    }

    private void Reset()
    {
        if (text != null)
            text.text = "No Message";
        OKFunction = null;
        isActive = false;
    }

    // Para ma-check kung active ang confirmation
    public bool IsConfirmationActive()
    {
        return isActive || isAnimating;
    }
}