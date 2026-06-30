using UnityEngine;
using TMPro;

public class CodeVerification : MonoBehaviour
{
    public ForgotPassword forgotpass;
    public TMP_InputField code;
    public GetEmail getEmail;
    private string uri = "/users/forgot/verify";

    public void Verify()
    {
        var data = new
        {
            email= forgotpass.email,
            code = code.text
        };

        // Call it properly using a lambda
        forgotpass.DisableInSeconds(() => formulate(data), 1.5f);
    }

    public void Resend()
    {
        getEmail.resend();
    }

    public void Cancel()
    {
        forgotpass.Reset();
    }

    private void formulate(object data)
    {
        forgotpass.FormulateRequest(data, uri, Success, Failed);
    }

    private void Failed()
    {
        print("failed");
        forgotpass.PrinErrors();
        if(forgotpass.GetSessionResponseMessage() == "Attempt Exceeded")
        {
            forgotpass.Reset();
        }
    }

    private void Success()
    {
        forgotpass.PrintMessage(
            "The Verification is Successfull You can now Change your password within 10 minutes",
            () => { forgotpass.ActivatePanel(FpassState.reset); }
        );

        code.text = "";
    }
}
