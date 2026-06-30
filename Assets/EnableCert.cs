using UnityEngine;
using UnityEngine.UI;

public class EnableCert : MonoBehaviour
{
    private void FixedUpdate()
    {
        gameObject.GetComponent<Button>().enabled = PlayerPrefs.GetInt("hasCertificate", 0) == 1;
    }
}
