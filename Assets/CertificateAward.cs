using UnityEngine;

public class CertificateAward : MonoBehaviour
{
    private PlayerLevelsData levelData;
    private bool hasCertificate;
    [SerializeField] private GameObject certificate;

    private void Start()
    {
        //PlayerPrefs.SetInt("hasCertificate", 0);
        levelData = PlayerLevelsData.LevelsData();
        hasCertificate = PlayerPrefs.GetInt("hasCertificate", 0) == 1;
        //print(PlayerLevelsData.LevelsData().noli + PlayerLevelsData.LevelsData().elfili);
        if (hasCertificate)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(levelData.IsCompleted() && !hasCertificate)
        {
            hasCertificate = true;
            PlayerPrefs.SetInt("hasCertificate", 1);
            //pop certificate
            certificate.SetActive(true);
        }
    }

}
