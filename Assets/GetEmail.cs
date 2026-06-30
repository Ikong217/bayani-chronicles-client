using UnityEngine;
using TMPro;

public class GetEmail : MonoBehaviour
{
    public ForgotPassword forgotpass;
    public TMP_InputField email;

    private string uri = "/users/forgot/request-code";

    public void RequestData()
    {
        var data = new
        {
            email = email.text.Trim()
        };

        // Call it properly using a lambda
        forgotpass.DisableInSeconds(() => formulate(data), 1.5f);
    }

    public void resend()
    {
        var data = new
        {
            email = forgotpass.email
        };

        // Call it properly using a lambda
        forgotpass.DisableInSeconds(() => formulate(data), 1.5f);
    }

    private void formulate(object data)
    {
        forgotpass.FormulateRequest(data, uri, Success, Failed);
    }

    private void Failed()
    {
        forgotpass.PrinErrors();
    }

    private void Success()
    {
        forgotpass.PrintMessage(
            "Please check your email for the verification code.",
            () => { forgotpass.ActivatePanel(FpassState.verify); }
        );
        forgotpass.email = email.text.Trim();

        email.text = "";
    }
}
