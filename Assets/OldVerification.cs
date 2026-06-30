using UnityEngine;
using TMPro;
using System.Collections;

public class OldVerification : MonoBehaviour
{
    [SerializeField] private ChangeEmail changeEm;
    [SerializeField] private TextMeshProUGUI email;
    [SerializeField] private TextMeshProUGUI code;

    private string uri = "/api/user/request/change/email/awaitcode";

    private void OnEnable()
    {
        email.text = MyData.Load().email;
        code.text = changeEm.GetCode();
        AwaitResponse();
    }

    public void AwaitResponse()
    {
        // Prevent spamming
        changeEm.DisableInSeconds(PrepareData, 2);
    }

    private void PrepareData()
    {
        string user_id = MyData.Load().user_id;

        var data = new
        {
            user_id = user_id,
        };

        changeEm.FormulateRequest(data, uri, Success, Fail);
    }

    private void Success()
    {
        changeEm.PrintMessage(changeEm.GetMessage(), () =>
            changeEm.ActivatePanel(ChangeEmail.Phase.NewCode));
    }

    private void Fail()
    {
        // Handle “On Process” loop
        if (changeEm.GetMessage() == "On Process")
        {
            StartCoroutine(RetryAwaitResponse());
        }
        else
        {
            changeEm.PrintErrors(() => changeEm.Quit());
        }
    }

    private IEnumerator RetryAwaitResponse()
    {
        yield return new WaitForSeconds(3f);
        AwaitResponse();
    }
}
