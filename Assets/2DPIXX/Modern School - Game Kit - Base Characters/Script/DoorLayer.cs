using UnityEngine;

public class DoorLayer : MonoBehaviour
{
    private SpriteRenderer sprite;
    private int originalOrderInLayer;  // Changed from float to int (OrderInLayer is int)
    private string originalLayer;
    private SetLocationManager setLoc;
    private SpriteRenderer mySprite;
    private GameObject player;
    private CharacterMovement movement;
    private bool changed = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            Debug.LogError("SpriteRenderer component missing on this GameObject!", this);
            enabled = false; // Disable the script if no SpriteRenderer is found
            return;
        }
        originalOrderInLayer = sprite.sortingOrder;
        originalLayer = sprite.sortingLayerName;// Correct method name
        setLoc = SetLocationManager.Instance;
        mySprite = gameObject.GetComponent<SpriteRenderer>();
        mySprite.sortingLayerName = "Default";
        player = Player.FindPlayer();
        movement = player.GetComponent<CharacterMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (setLoc == null) Debug.LogWarning("No Setlocation Active");
        if (collision.CompareTag("Player"))
        {
            if (collision == setLoc.GetCharacter())
            {
                mySprite.sortingLayerName = "Wall";
                SpriteRenderer playerSprite = collision.GetComponent<SpriteRenderer>();
                if (playerSprite != null)
                {
                    sprite.sortingOrder = playerSprite.sortingOrder + 1;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //print("andito");
        
        if (collision.CompareTag("Player"))
        {
            //SpriteRenderer playerSprite = collision.GetComponent<SpriteRenderer>();
            //if (playerSprite != null)
            //{
                //print("natapakan");
                sprite.sortingOrder = 10;
                sprite.sortingLayerName = "Player";
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //print("umalis");
            sprite.sortingOrder = originalOrderInLayer;
            sprite.sortingLayerName = originalLayer;
            mySprite.sortingLayerName = "Default";
        }
    }
    private void FixedUpdate()
    {
        if (movement.onGoingEvent && player == setLoc.GetCharacter())
        {
            sprite.sortingOrder = 10;
            sprite.sortingLayerName = "Player";
            changed = true;
        }
        else if(changed == true)
        {
            sprite.sortingOrder = originalOrderInLayer;
            sprite.sortingLayerName = originalLayer;
            mySprite.sortingLayerName = "Default";
            changed = false;
        }
    }
}