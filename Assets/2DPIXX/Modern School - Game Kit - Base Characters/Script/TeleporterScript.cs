using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    public Transform teleportLocation;

    private void OnCollisionEnter2D(Collision2D collision)
    {
            print("Triggered, Player");
        if(collision.gameObject.CompareTag("Player"))
        {
            print("Teleported, Player");
            GameObject player = collision.gameObject;
            Teleport(player);
        }
        //else if (collision.gameObject.CompareTag("Offset"))
        //{
        //    GameObject player = collision.gameObject;
        //    Teleport(player);
        //}
    }

    public void Teleport(GameObject player)
    {
            player.transform.position = teleportLocation.transform.position;
    }
}
