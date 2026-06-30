using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ChangePass : MonoBehaviour
{
    public ForgotPassword forgotpass;
    public TMP_InputField pass;
    public TMP_InputField cpass;
    private string uri = "/users/reset/submit";

    public void Change()
    {
        //print(forgotpass.code);
        string password = pass.text;
        string confirm = cpass.text;
        string code = forgotpass.code.Replace("reset_", "");

        // Validate input
        if (!IsStrongPassword(password))
        {
            forgotpass.PrintWarningMessage("You need a stronger password. It must contain uppercase, lowercase, number, and symbol.");
            return;
        }

        if (password != confirm)
        {
            forgotpass.PrintWarningMessage("Password and confirmation do not match.");
            return;
        }

        var data = new
        {
            code = code,
            password = password,
            password_confirmation = confirm
        };

        // Call it properly using a lambda
        forgotpass.DisableInSeconds(() => Formulate(data), 1.5f);
    }

    private void Formulate(object data)
    {
        forgotpass.FormulateRequest(data, uri, Success, Failed);
    }

    private void Failed()
    {
        Debug.LogError("Failed to reset password");
        forgotpass.PrinErrors();
    }

    private void Success()
    {
        forgotpass.PrintMessage(
            "Your Password has successfully Saved",
            () => { forgotpass.Reset(); }
        );

        pass.text = "";
        cpass.text = "";
    }

    // Password Strength Validation Function
    private bool IsStrongPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // At least 1 upper, 1 lower, 1 digit, 1 special char, and 8+ chars long
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$";
        return Regex.IsMatch(password, pattern);
    }
}
