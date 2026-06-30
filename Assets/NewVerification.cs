using UnityEngine;
using TMPro;

public class NewVerification : MonoBehaviour
{
    [SerializeField] private ChangeEmail changeEm;
    [SerializeField] private TMP_InputField codeInp;

    private string user_id;
    private string uri = "/api/user/request/change/email/verify";

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        codeInp.text = string.Empty;
        user_id = MyData.Load().user_id; // hashed, stored as string
    }

    public void OnPressedSubmit()
    {
        // Prevent spamming — calls PrepareData() and disables for 2s
        changeEm.DisableInSeconds(PrepareData, 2);
    }

    private void PrepareData()
    {
        string code = codeInp.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            changeEm.PrintWarningMessage("Please enter the code.");
            return;
        }

        var data = new
        {
            user_id = user_id,
            code = code
        };

        changeEm.FormulateRequest(data, uri, Success, Fail);
    }

    private void Success()
    {
        Reset();
        changeEm.PrintMessage(changeEm.GetMessage(), () =>
        {
            MyData data = MyData.Load();
            data.email = changeEm.GetMyData().email;
            data.SaveAll();

            ShowProfileScript.Instance.UpdateUserData();
            changeEm.Quit();
        });
    }

    private void Fail()
    {
        changeEm.PrintErrors();

        if (changeEm.GetMessage() == "expired")
        {
            changeEm.Quit();
        }
    }
}
