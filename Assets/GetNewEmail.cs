using UnityEngine;
using TMPro;

public class GetNewEmail : MonoBehaviour
{
    [SerializeField] private ChangeEmail changeEm;
    [SerializeField] private TMP_InputField chEmInp;

    private string user_id;
    private string uri = "/api/user/request/change/email";

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        chEmInp.text = string.Empty;
        user_id = MyData.Load().user_id; // hashed, stored as string
    }

    public void OnPressedSubmit()
    {
        // prevent spamming — calls PrepareData() and disables for 2s
        changeEm.DisableInSeconds(PrepareData, 2);
    }

    private void PrepareData()
    {
        string email = chEmInp.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            changeEm.PrintWarningMessage("Please enter your new email address.");
            return;
        }

        var data = new
        {
            user_id = user_id,
            new_email = email
        };

        changeEm.FormulateRequest(data, uri, Success, Fail);
    }

    private void Success()
    {
        Reset();
        changeEm.PrintMessage(changeEm.GetMessage(),()=>changeEm.ActivatePanel(ChangeEmail.Phase.OldCode));
    }

    private void Fail()
    {
        changeEm.PrintErrors();
    }
}
