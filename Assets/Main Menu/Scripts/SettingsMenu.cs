using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

	public GameObject SoundToggle;
	public Text[] Labels;

	//PlayerPrefs has the Quality Level and whether or not the Sound was muted
	//So we'll set it to what it was before!
	void Start () 
	{
		if (PlayerPrefs.GetInt ("SoundToggle") == 0)
			SoundToggle.SetActive (true);
		else
			SoundToggle.SetActive (false);
		
		AdjustVolume (SoundToggle.activeSelf);

        if (!PlayerPrefs.HasKey("QualityLevel"))
            PlayerPrefs.SetInt("QualityLevel", 5);
		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt ("QualityLevel"));
		ChangeQuality(PlayerPrefs.GetInt("QualityLevel"));
	}

	//Changes the quality, by default Unity has 6 quality levels so we'll use 0, 3, and 5!
	public void ChangeQuality(int index)
	{
		QualitySettings.SetQualityLevel (index);
		PlayerPrefs.SetInt ("QualityLevel", index);

		if (PlayerPrefs.GetInt ("QualityLevel") == 0)
			SetLabel (0);
		else if (PlayerPrefs.GetInt ("QualityLevel") == 3)
			SetLabel (1);
		else if (PlayerPrefs.GetInt ("QualityLevel") == 5)
			SetLabel (2);
	}

	//Global toggle mute to mute or unmute all sounds!
	public void ToggleSound()
	{
		if (PlayerPrefs.GetInt ("SoundToggle") == 0) 
        {
			PlayerPrefs.SetInt ("SoundToggle", 1);
			SoundToggle.SetActive (false);
		} 
		else 
        {
			PlayerPrefs.SetInt ("SoundToggle", 0);
			SoundToggle.SetActive (true);
		}

		AdjustVolume (SoundToggle.activeSelf);
	}

	//Adjusts the volume based on what happens in "ToggleSound"
	private void AdjustVolume(bool setting)
	{
        if (setting)
			AudioListener.volume = 1;
		else
			AudioListener.volume = 0;
	}

	//Sets the color of the label that we have selected to show what is in use
	private void SetLabel(int index)
	{
		Labels [0].color = Color.white;
		Labels [1].color = Color.white;
		Labels [2].color = Color.white;
		Labels [index].color = new Color (0.55f, 0.81f, 0.38f);
	}
}