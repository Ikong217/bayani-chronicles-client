using UnityEngine;
using System.Collections;

[System.Serializable]
public class ScrollContent
{
    [Header("Title")]
    public string title;

    [Header("Image")]
    public Sprite sprite;
    public string imageDescription;

    [Header("Description")]
    [TextArea(5,20)]
    public string description;
}

public class ScrollShowTrigger : MonoBehaviour
{
    [Header("Contents of the Scrolls")]
    public ScrollContent scrollContent;

    [Header("In Game Mechanics")]
    public ObjectsContainer affectedObjects;
    public ObjectsContainer activatedObjects;

    private void TriggerEvents()
    {
        StartCoroutine(WaitForFinishCoroutine());
    }

    public void PubTrigger() => TriggerEvents();

    IEnumerator WaitForFinishCoroutine()
    {
        ScrollShowManager scrollShow = ScrollShowManager.Instance;
        ScrollContent content = scrollContent;

        scrollShow.StartScroll(content);

        while (!scrollShow.finished)
        {
            yield return null;
        }

        ObjectsHandler.DestroyObject(affectedObjects, gameObject);
        ObjectsHandler.ActivateObject(activatedObjects);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActiveAndEnabled)
            return;

        if (!Player.FindPlayer().GetComponent<CharacterMovement>().onGoingEvent)
        {
            if (collision.CompareTag("Offset"))
            {
                //TriggerDialogue();
                TriggerEvents();
            }
        }
    }
}