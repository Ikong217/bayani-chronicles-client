using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.ondad.alertpanels;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{

	public GameObject[] LevelButtons;
	public Image FadeScreen;
	public AudioSource source;
	public Novels initialNovel = Novels.NoliMeTangere;
	public GameObject summativeButton;
	private string touchLevel = "";
	private int count = 0;

	//Going to call "SetUnlockedLevels" in start to show the Lock overlay for levels we have not gone to yet!
	void Start()
	{
		//SetUnlockedLevels ();
		touchLevel = "";
		count = 0;
	}

	//Checks the PlayerPref being passed in when the button is clicked to see if we unlocked the level
	//If the level is unlocked we will load the scene with that name
	//NOTE: These scene load will only work if there is a scene named that in the "Scenes In Build"
	public void LoadScene(PlayerLevels lvl)
	{
		if (!lvl.isLocked)
		{
			// Add your fade + scene load here
			SimpleUI.Instance.FadeOut(FadeScreen, 1.5f);
			SimpleUI.Instance.LoadLevelDelay(lvl.Levelname, 1.5f); ;
		}
		else
		{
			Debug.Log("Level locked: " + lvl.Levelname);
		}
	}


	//Turns off the Lock overlay for all unlocked levels!
	//We're setting Level1 to unlocked so we have a level to play!
	private void SetUnlockedLevels(Novels novel)
	{
		//Set Level1 to unlocked
		//PlayerPrefs.SetInt ("Level1", 1);
		//PlayerPrefs.SetInt ("Level2", 1);
		//for (int i = 0; i < LevelButtons.Length; i++) 
		//{
		//	if(PlayerPrefs.GetInt("Level"+(i+1)) == 1)
		//    {
		//		LevelButtons[i].transform.Find("Lock").gameObject.SetActive (false);
		//	}
		//}

		PlayerNovels pressedNovel = PlayerLevelsManager.Load(novel);

		PlayerLevels[] levels = GetPlayerLevels(pressedNovel);

		for (int i = 0; i < LevelButtons.Length; i++)
		{
			LevelButtons[i].transform.Find("Lock").gameObject.SetActive(true);

			if (i <= levels.Length - 1)
            {
				LVLButtonStarIcon stars = LevelButtons[i].GetComponent<LVLButtonStarIcon>();
				stars.SetStars(levels[i].stars);
				//print(levels[i].stars);

				if (!levels[i].isLocked)
				{
					//print(levels[i].isLocked);
					LevelButtons[i].transform.Find("Lock").gameObject.SetActive(false);
				}
			}

		}

		summativeButton.transform.Find("Lock").gameObject.SetActive(true);
        if (PlayerLevelsManager.Load(novel).IsFinished())
        {
			summativeButton.transform.Find("Lock").gameObject.SetActive(false);
		}

	}//babaguhin ko to pag kumpleto na levels natin

	public void SetAvailableLevels(Novels novel)
	{
		PlayerNovels pressedNovel = PlayerLevelsManager.Load(novel);
		int levelCount = pressedNovel.playerLevels.Count;
		print(levelCount);
		if (levelCount > LevelButtons.Length)
			levelCount = LevelButtons.Length;

		if (LevelButtons.Length > 0 && levelCount <= 0)
			levelCount = 1;
		else if (LevelButtons.Length <= 0)
			return;

		//disabling gameobjects
		foreach (GameObject lvl in LevelButtons)
		{
			lvl.SetActive(false);
		}

		PlayerLevels[] levels = GetPlayerLevels(pressedNovel);

		//enabling how many available levels
		for (int i = 0; i < levelCount; i++)
		{
			LevelButtons[i].SetActive(true);
			Button btn = LevelButtons[i].GetComponent<Button>();
			btn.onClick.RemoveAllListeners();

			// ✅ Wrap in lambda so it doesn’t execute immediately
			btn.onClick.AddListener(() => source.Play());

			// ✅ Pass the PlayerLevels properly

			if(i <= levels.Length -1)
            {
				//print(levels[i].Levelname);
				PlayerLevels level = levels[i];
				btn.onClick.AddListener(() => LoadScene(level));

				//lagay notif pag ano hahhaa
				btn.onClick.AddListener(() => ClickedLevel(level.Levelname));
            }
		}

		summativeButton.SetActive(true);
		if (PlayerLevelsManager.Load(novel).IsFinished())
		{
			Button btn = summativeButton.GetComponent<Button>();
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() => source.Play());

			string direction = (novel == Novels.NoliMeTangere) ? "Noli_Summative" : "ElFi_Summative";

			btn.onClick.AddListener(() => SceneManager.LoadScene(direction));

			//lagay notif pag ano hahhaa
			btn.onClick.AddListener(() => ClickedLevel(direction));
		}

		SetUnlockedLevels(novel);
	}

	public PlayerLevels[] GetPlayerLevels(PlayerNovels playerNovels)
	{
		PlayerLevels[] playerLevels = new PlayerLevels[playerNovels.playerLevels.Count];
		for (int i = 0; i < playerNovels.playerLevels.Count; i++)
		{
			playerLevels[i] = playerNovels.playerLevels[i];
		}
		return playerLevels;
	}

	public void ClickedLevel(string level)
    {
		if(level != touchLevel)
        {
			touchLevel = level;
			count = 1;
        }
        else
        {
			count++;
			if(count >= 5)
            {
				AlertManager.GetInstance().ShowWarningPanel("This Level is Locked, Please finish the previous Level first!!");
				count = 3;
            }
        }
		print("Clicked: " + level + " | Count: " + count.ToString());
    }
}
