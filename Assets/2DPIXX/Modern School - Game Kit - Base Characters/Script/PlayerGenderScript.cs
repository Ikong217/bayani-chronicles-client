using UnityEngine;
using System.Collections;

public class PlayerGenderScript : MonoBehaviour
{
    public GameObject MaleCharacter;
    public GameObject FemaleCharacter;
    public Transform StartingPosition;
    private Camera cam;
    private CamFollow camscr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayerPrefs.SetString("gender", "male");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        camscr = cam.GetComponent<CamFollow>();
        //print(camscr);

        string gender = PlayerPrefs.GetString("gender");
        //gender = "male";

        gender = gender.ToLower();

        if(StartingPosition != null)
        {
            MaleCharacter.transform.position = StartingPosition.position;
            FemaleCharacter.transform.position = StartingPosition.position;
        }

        if(gender == "female")
        {
            Destroy(MaleCharacter.gameObject);
            StartCoroutine(CamFollow(FemaleCharacter.transform));
        }
        else
        {
            Destroy(FemaleCharacter.gameObject);
            StartCoroutine(CamFollow(MaleCharacter.transform));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator CamFollow(Transform position)
    {
        yield return null;
        //print(position);
        camscr.GoToTargetPosition(position);
    }
}
