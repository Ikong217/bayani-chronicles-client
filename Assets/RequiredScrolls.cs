using UnityEngine;

public class RequiredScrolls : MonoBehaviour
{
    [SerializeField] private ScrollCounter scrollCounter;

    [SerializeField] private int requiredScrolls;
    [SerializeField] private ObjectsContainer affectedObj;
    [SerializeField] private ObjectsContainer activateObj;

    private void Update()
    {
        if(scrollCounter == null)
        {
            Debug.LogWarning("No Scroll counter Available");
            return;
        }

        if(scrollCounter.GetScrollsCount() >= requiredScrolls)
        {
            ObjectsHandler.ActivateObject(activateObj);
            ObjectsHandler.DestroyObject(affectedObj);

            Destroy(gameObject);
        }
    }
}
