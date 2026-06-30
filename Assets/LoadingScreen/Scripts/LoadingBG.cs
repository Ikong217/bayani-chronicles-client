using UnityEngine;
using System.Collections;

public class LoadingBG : MonoBehaviour
{
    public bool backGroundImageAndLoop = false;
    public GameObject[] backgroundImages;
    public float LoopTime = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backGroundImageAndLoop = true;
        if (backGroundImageAndLoop && backgroundImages.Length > 0)
        {
            StartCoroutine(transitionImage());
        }
    }

    IEnumerator transitionImage()
    {
        while (true)
        {
            for (int i = 0; i < backgroundImages.Length; i++)
            {
                yield return new WaitForSeconds(LoopTime);

                for (int j = 0; j < backgroundImages.Length; j++)
                    backgroundImages[j].SetActive(false);

                //print("changed");

                backgroundImages[i].SetActive(true);
            }
        }
    }

}
