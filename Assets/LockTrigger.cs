using UnityEngine;

public class LockTrigger : MonoBehaviour
{
    private LockMechanism lockMech;

    [SerializeField] private ObjectsContainer activatedObjects;
    [SerializeField] private ObjectsContainer destroyObjects;

    [Header("Ignore if no change needed")]
    [SerializeField] private string password;

    private void Start()
    {
        lockMech = LockMechanism.GetInstance();
        lockMech.PlayCreate(Ending, password);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision);
        if (collision.CompareTag("Offset") )
        {
            lockMech.Play();
        }
    }

    private void Ending()
    {
        ObjectsHandler.ActivateObject(activatedObjects);
        ObjectsHandler.DestroyObject(destroyObjects,gameObject);
        Destroy(gameObject);
    }
}
