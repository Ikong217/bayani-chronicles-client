using UnityEngine;
using UnityEngine.SceneManagement;

public class BannedPanelScript : MonoBehaviour
{
    public string loginSceneName;

    public void OKPressed()
    {
        // Corrected method name
        SceneManager.LoadScene(loginSceneName);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
