using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;

public class LockMechanism : MonoBehaviour
{
    private static LockMechanism instance;

    [SerializeField] private List<TMP_InputField> fields;

    [Header("Fixed password 11111")]
    [SerializeField] private bool isFixed = false;

    [Header("Containers to be poped")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject container; 

    private string password;
    private Action onUnlock;
    private void Awake()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        SetupFields();
        Randomize();
    }

    private void SetupFields()
    {
        if (fields.Count < 5)
        {
            Debug.LogWarning("Not enough TMP_InputFields assigned.");
            return;
        }

        MarkFieldsColor(Color.white);

        foreach (TMP_InputField field in fields)
        {
            field.text = "";
            field.characterLimit = 1;
            field.onValueChanged.RemoveAllListeners();
            field.onValueChanged.AddListener(delegate { ValidateField(field); });
        }
    }

    private void Randomize()
    {
        if (isFixed)
        {
            password = "11111";
            return;
        }

        int randInt = UnityEngine.Random.Range(0, 99999);
        password = randInt.ToString("D5"); // always 5 digits
        //print(password);
    }

    //handle backspace enters
    private void Update()
    {
        // Check if backspace is pressed
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            GameObject current = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (current != null)
            {
                TMP_InputField field = current.GetComponent<TMP_InputField>();

                if (field != null)
                {
                    OnBackspace(field);
                }
            }
        }
    }

    private void OnBackspace(TMP_InputField field)
    {
        int index = fields.IndexOf(field);

        if (index <= 0) return; // can't go back from first

        // remove current field content
        if (field.text.Length > 0)
        {
            field.text = "";
            return;
        }

        // if current is empty, go back one slot
        TMP_InputField prev = fields[index - 1];
        prev.text = "";
        prev.Select();
        prev.ActivateInputField();
    }


    public void ValidateField(TMP_InputField field)
    {
        MarkFieldsColor(Color.white);
        int index = fields.IndexOf(field);

        if (index == -1)
        {
            Debug.LogWarning("Field not part of the lock.");
            return;
        }

        // enforce 1 char only
        if (field.text.Length > 1)
            field.text = field.text.Substring(0, 1);

        // If last field filled → check password
        if (IsAllFieldsFilled())
        {
            string userInput = GetEnteredPassword();

            if (userInput == password)
            {
                MarkFieldsColor(Color.green);
                StartCoroutine(UnlockDelay());
            }
            else
            {
                MarkFieldsColor(Color.red);
            }
        }

        // Move to the next field
        if (field.text.Length == 1 && index < fields.Count - 1)
        {
            fields[index + 1].Select();
            fields[index + 1].ActivateInputField();
            return;
        }

    }

    private bool IsAllFieldsFilled()
    {
        foreach (TMP_InputField f in fields)
        {
            if (f.text.Trim().Length == 0)
                return false;
        }
        return true;
    }

    private string GetEnteredPassword()
    {
        string s = "";
        foreach (TMP_InputField f in fields)
            s += f.text;
        return s;
    }

    private void MarkFieldsColor(Color c)
    {
        foreach (TMP_InputField f in fields)
        {
            var colors = f.colors;
            colors.normalColor = c;
            colors.selectedColor = c;
            colors.highlightedColor = c;
            f.colors = colors;
        }
    }

    private IEnumerator UnlockDelay()
    {
        yield return new WaitForSeconds(1.5f);
        onUnlock?.Invoke();
    }

    public static LockMechanism GetInstance()
    {
        //print(instance);

        return instance;
    }

    public LockMechanism SetNewLock(Action action, bool isFixedPassword = false)
    {
        onUnlock = action;
        isFixed = isFixedPassword;

        SetupFields();
        Randomize();

        return this;
    }

    public void Play()
    {
        if (background == null)
        {
            Debug.LogWarning("Password:: Missing Background");
            return;
        }

        if (container == null)
        {
            Debug.LogWarning("Password:: Missing Container");
            return;
        }

        background.SetActive(true);
        container.SetActive(true);

        foreach (TMP_InputField f in fields)
        {
            f.text = "";
        }
    }
    
    public void Back()
    {
        if(background == null)
        {
            Debug.LogWarning("Password:: Missing Background");
            return;
        }

        if(container == null)
        {
            Debug.LogWarning("Password:: Missing Container");
            return;
        }

        background.SetActive(false);
        container.SetActive(false);
    }

    public void PlayCreate(Action action, string password = null)
    {
        onUnlock = action;
        if (!string.IsNullOrEmpty(password))
        {
            Randomize();
        }
    }

    public string GetPassword()
    {
        return password;
    }
}
