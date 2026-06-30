using UnityEngine;
using System.Collections;

public class ConfirmationMenu : MonoBehaviour {

	//Do what you want when the user presses the "YES" button, in this case we're quitting
	//The "NO" button is already setting the gameobject active to false on the button event
	public void YesButton()
	{
		Application.Quit ();
	}
}
