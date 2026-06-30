using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleUI : MonoBehaviour 
{
    public static SimpleUI _instance;
    public static SimpleUI Instance
    {
        get
        {
            if (!_instance)
            {
                // Using the updated method to find the instance
                _instance = Object.FindFirstObjectByType<SimpleUI>();
                
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "SimpleUIObject";
                    _instance = container.AddComponent<SimpleUI>();
                }
            }

            return _instance;
        }
    }

    //Scales an object to a given scale over a given duration
    public void ScaleTo(GameObject obj, Vector3 endScale, float duration) 
    {
        StartCoroutine(ScaleObj(obj, endScale, duration));
    }

    //Loads the given level after a delay
    public void LoadLevelDelay(string name, float duration)
    {
        StartCoroutine(Load(name, duration));
    }

    //Fades in an image over a given duration
    public void FadeIn(Image img, float duration)
    {
        StartCoroutine(Fade(true, img, duration));
    }

    //Fades out an image over a given duration
    public void FadeOut(Image img, float duration)
    {
        StartCoroutine(Fade(false, img, duration));
    }

    //Coroutine to scale the object to the target size over time
    IEnumerator ScaleObj(GameObject obj, Vector3 endScale, float duration)
    {
        obj.SetActive(true);
        float progress = 0.0f;
        while (progress <= duration)
        {
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, endScale, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = endScale;
        if (obj.transform.localScale.x <= 0)
            obj.SetActive(false);
    }

    //Coroutine to fade in or fade out the image
    IEnumerator Fade(bool bIn, Image img, float duration)
    {
        float startTime = Time.time;
        float timePassed = 0.0f;
        Color endColor;

        if (bIn)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1.0f); 
            endColor = new Color(img.color.r, img.color.g, img.color.b, 0.0f); 
        }
        else
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.0f); 
            endColor = new Color(img.color.r, img.color.g, img.color.b, 1.0f); 
        }

        while (timePassed < duration)
        { 
            timePassed = Time.time - startTime;
            float nT = Mathf.Clamp(timePassed / duration, 0, 1);
            img.color = Color.Lerp(img.color, endColor, nT);
            yield return new WaitForFixedUpdate(); 
        } 
    }

    //Coroutine to load a scene after the given delay
    IEnumerator Load(string name, float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(name);
    }
}
