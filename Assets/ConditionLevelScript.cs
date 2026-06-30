using UnityEngine;

public class ConditionLevelScript : MonoBehaviour
{
    private ScrollCounter scrollCounter;
    [SerializeField] private ObjectsContainer deactivate;
    [SerializeField] private ObjectsContainer activate;

    private void Start()
    {
        GameObject scrollCounterObj = GameObject.Find("ScrollCounter");
        if (scrollCounterObj != null)
        {
            scrollCounter = scrollCounterObj.GetComponent<ScrollCounter>();
            if (scrollCounter == null)
                Debug.LogError("Missing ScrollCounter component on: " + scrollCounterObj.name);
        }
        else
        {
            Debug.LogError("Could not find GameObject named: ScrollCounter");
        }
    }

    private void Update()
    {
        if (scrollCounter.GetScrollsCount() == 9)
        {
            ObjectsHandler.ActivateObject(activate);
            ObjectsHandler.DestroyObject(deactivate);
            Destroy(gameObject);
        }
    }
}
