using UnityEngine;

public class SubObj : MonoBehaviour
{
    [SerializeField] private GameObject sourceObj;
    private bool initialized = false;

    private void OnEnable()
    {
        if (initialized) return;
        initialized = true;

        // Make sure sourceObj is valid before accessing it
        if (sourceObj == null)
        {
            Debug.LogWarning($"[SubObj] Source object is missing on {name}");
            Destroy(this);
            return;
        }

        transform.position = sourceObj.transform.position;

        try
        {
            var sourceAnim = sourceObj.GetComponent<CharAnimation>();
            var thisAnim = GetComponent<CharAnimation>();
            //print(sourceAnim.GetCurrentDirection());
            if (sourceAnim != null && thisAnim != null)
            {
                DirectionType direction = sourceAnim.GetCurrentDirection();
                thisAnim.SetDirection(direction);
                print(direction);
            }
            else
            {
                print("No This Anim");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[SubObj] Error copying animation: {e.Message}");
        }

        // Self-destruct after setup
        Destroy(this);
    }
}
