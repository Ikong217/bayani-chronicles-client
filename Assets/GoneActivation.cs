using UnityEngine;
using System.Collections;

public class GoneActivation : MonoBehaviour
{
    [Header("Gameobjects are Gone To activate")]
    [SerializeField] ObjectsContainer requiredDeleted;

    [Header("Gameobjects To Be Activated")]
    [SerializeField] ObjectsContainer activatedObjects;

    [Header("Gameobjects To Be Destroyed")]
    [SerializeField] ObjectsContainer destroyObjects;

    private void Deact() => gameObject.SetActive(false);

    private void Awake() => Deact();

    private void OnEnable()
    {
        StartCoroutine(WaitTillVerify());
    }

    private void StartAction()
    {
        print("Activated");
        foreach(GameObject obj in requiredDeleted.container)
        {
            if(obj != null)
            {
                Deact();
                print("Failed: ActiveScroll is" + obj.name);
                return;
            }
        }

        print("success");

        ObjectsHandler.ActivateObject(activatedObjects);
        ObjectsHandler.DestroyObject(destroyObjects);
        Destroy(gameObject);

    }

    IEnumerator WaitTillVerify(float time = 1.5f)
    {
        yield return new WaitForSeconds(time);
        yield return null;//skips a frame to make sure
        StartAction();
    }

}
