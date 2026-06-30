using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    /*
	public GameObject LevelSelectMenu;
	public GameObject SettingsMenu;
	public GameObject CreditsMenu;
    public GameObject ConfirmationMenu;
    public GameObject HomeButton;
    public Image FadeScreen;

    public void Awake()
    {
        SimpleUI.Instance.FadeIn(FadeScreen, 1.5f);
    }

	public void PlayButtonPressed()
	{
        SimpleUI.Instance.ScaleTo(LevelSelectMenu, new Vector3(1,1,1), 0.5f);
	}

	public void SettingsButtonPressed()
	{
        SimpleUI.Instance.ScaleTo(SettingsMenu, new Vector3(1,1,1), 0.5f);
	}

	public void CreditsButtonPressed()
	{
        SimpleUI.Instance.ScaleTo(CreditsMenu, new Vector3(1,1,1), 0.5f);
	}

	public void ExitButtonPressed()
	{
        SimpleUI.Instance.ScaleTo(ConfirmationMenu, new Vector3(1,1,1), 0.5f);
	}

    public void BoxOutButtonPressed()
    {
        if (LevelSelectMenu.activeSelf)
            SimpleUI.Instance.ScaleTo(LevelSelectMenu, new Vector3(0.001f, 0.001f, 0.001f), 0.25f); // Scale to a small value instead of 0

        if (SettingsMenu.activeSelf)
            SimpleUI.Instance.ScaleTo(SettingsMenu, new Vector3(0.001f, 0.001f, 0.001f), 0.25f);

        //if (CreditsMenu.activeSelf)
        //    SimpleUI.Instance.ScaleTo(CreditsMenu, new Vector3(0.001f, 0.001f, 0.001f), 0.25f);

        if (ConfirmationMenu.activeSelf)
            SimpleUI.Instance.ScaleTo(ConfirmationMenu, new Vector3(0.001f, 0.001f, 0.001f), 0.25f);
    }


    public void HomeButtonPressed()
    {
        SimpleUI.Instance.FadeOut(FadeScreen, 1.5f);
        SimpleUI.Instance.LoadLevelDelay("DemoScene", 1.5f);
    }
    */



    public Image FadeScreen;

    public GameObject NovelSelectMenu;
    public GameObject LevelSelectMenu;
    private LevelMenu levelMenu;

    public GameObject InventoryMenu;
    public GameObject GuideMenu;
    public GameObject ProfileMenu;
    public GameObject ConfirmationMenu;
    public GameObject ProfileConfirmationMenu;
    public GameObject ScrollMenu;
    public GameObject LeaderboardMenu;

    private GameObject activePanel =  null;
    public void Awake()
    {
        SimpleUI.Instance.FadeIn(FadeScreen, 1.5f);
        levelMenu = LevelSelectMenu.GetComponent<LevelMenu>();
        if (levelMenu == null)
            Debug.LogError("Missing LevelMenu");
    }

    public void PlayButtonPressed()
    {
        SimpleUI.Instance.ScaleTo(NovelSelectMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = NovelSelectMenu;
    }

    public void NoliButtonPressed()
    {
        BoxOutButtonPressed();
        SimpleUI.Instance.ScaleTo(LevelSelectMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = LevelSelectMenu;
        levelMenu.SetAvailableLevels(Novels.NoliMeTangere);
    }

    public void ElFiliButtonPressed()
    {
        BoxOutButtonPressed();
        SimpleUI.Instance.ScaleTo(LevelSelectMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = LevelSelectMenu;
        levelMenu.SetAvailableLevels(Novels.ElFilibusterismo);
    }

    public void ExitLevelPanel()
    {
        BoxOutButtonPressed();
        SimpleUI.Instance.ScaleTo(NovelSelectMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = NovelSelectMenu;
    }

    public void InventoryButtonPressed()
    {
        BoxOutButtonPressed();
        SimpleUI.Instance.ScaleTo(InventoryMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = InventoryMenu;
    }

    public void GuideButtonPressed()
    {
        SimpleUI.Instance.ScaleTo(GuideMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = GuideMenu;
    }
    public void LeaderboarButtonPressed()
    {
        SimpleUI.Instance.ScaleTo(LeaderboardMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = LeaderboardMenu;
    }

    public void ProfileButtonPressed()
    {
        SimpleUI.Instance.ScaleTo(ProfileMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = ProfileMenu;
    }

    public void ExitButtonPressed()
    {
        SimpleUI.Instance.ScaleTo(ConfirmationMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = ConfirmationMenu;
    }

    public void QuitButtonPressed()
    {
        BoxOutButtonPressed();
        SimpleUI.Instance.ScaleTo(ProfileConfirmationMenu, new Vector3(1, 1, 1), 0.5f);
        activePanel = ProfileConfirmationMenu;
    }

    public void ScrollMenuPressed(string title)
    {
        BoxOutButtonPressed();
        SimpleUI.Instance.ScaleTo(ScrollMenu, new Vector3(1, 1, 1), 0.5f);
        ScrollMenuScript scrollMenuScript = ScrollMenu.GetComponent<ScrollMenuScript>();
        scrollMenuScript.SetScrollMenu(title);
        activePanel = ScrollMenu;
    }

    public void ToGuideClick()
    {
        SceneManager.LoadScene("Guide");
    }
    public void BoxOutButtonPressed()
    {
        if (activePanel == null)
            return;

        if (activePanel.activeSelf)
        {
            SimpleUI.Instance.ScaleTo(activePanel, new Vector3(0.001f, 0.001f, 0.001f), 0.25f);
            activePanel = null;
        }
    }

}
