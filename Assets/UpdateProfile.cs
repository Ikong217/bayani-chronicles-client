using UnityEngine;

public class UpdateProfile : MonoBehaviour
{
    public GameObject username;
    public GameObject email;
    [SerializeField] private MainMenu mm;

    public void ActivateUsername()
    {
        username.SetActive(true);
        mm.BoxOutButtonPressed();
    }

    public void ActivateEmail()
    {
        email.SetActive(true);
        mm.BoxOutButtonPressed();
    }
}
