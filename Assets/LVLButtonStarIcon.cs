using UnityEngine;

public class LVLButtonStarIcon : MonoBehaviour
{
    public void SetStars(int star)
    {
        // Find parent object safely
        Transform parentTransform = transform.Find("Interactablestars");
        if (parentTransform == null)
        {
            Debug.LogError($"[{name}] Missing parent: 'Interactablestars' : " + gameObject.name);
            return;
        }

        GameObject parent = parentTransform.gameObject;

        // Find each star safely
        Transform star1T = parentTransform.Find("1stars");
        Transform star2T = parentTransform.Find("2stars");
        Transform star3T = parentTransform.Find("3stars");

        if (star1T == null || star2T == null || star3T == null)
        {
            if (star1T == null) Debug.LogError($"[{name}] Missing child: '1stars'");
            if (star2T == null) Debug.LogError($"[{name}] Missing child: '2stars'");
            if (star3T == null) Debug.LogError($"[{name}] Missing child: '3stars'");
            return;
        }

        GameObject star1 = star1T.gameObject;
        GameObject star2 = star2T.gameObject;
        GameObject star3 = star3T.gameObject;

        // Ensure parent and stars are active
        parent.SetActive(true);
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);

        // Enable based on star count
        if (star > 0) star1.SetActive(true);
        if (star > 1) star2.SetActive(true);
        if (star > 2) star3.SetActive(true);
    }
}
