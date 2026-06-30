using UnityEngine;

public class UiAccountManager : MonoBehaviour {
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
	

	// Use this for initialization
	void Awake () {
        ClearPlayerData();
	}

    private void ClearPlayerData()
    {
        PlayerPrefs.DeleteAll();
    }
	void Start () {
	
	}

    public void OpenLogin()
    {

        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true) ;
    }

    public void OpenRegister()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

}
