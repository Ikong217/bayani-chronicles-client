using UnityEngine;

public class ItemLayeringScript : MonoBehaviour
{

    private bool isMePlayer = false;
    private Transform playerTransform;
    private SpriteRenderer meSprite;
    private int currentID;
    private string currentLayer;
    public float offset = 0.5f;
    private int layeringOrder = 0;
    private bool isOn = true;

    private void Start()
    {
        isMePlayer = Player.FindPlayer() == gameObject ? true : false;
        playerTransform = Player.FindPlayer().transform;
        if (!isMePlayer)
        {
            meSprite = gameObject.GetComponent<SpriteRenderer>();
            currentID = meSprite.sortingOrder;
            currentLayer = meSprite.sortingLayerName;
            layeringOrder = meSprite.sortingOrder;
        }
    }
    private void Update()
    {
        if(isOn)
            Layering();
    }


    private void Layering()
    {
        if (!isMePlayer)
        {
            if (playerTransform.position.y + offset > transform.position.y)
            {
                meSprite.sortingOrder = 10 + layeringOrder;
                meSprite.sortingLayerName = "Player";
            }
            else
            {
                meSprite.sortingOrder = currentID;
                meSprite.sortingLayerName = currentLayer;
            }
        }
    }

    public void TurnOn()
    {
        isOn = true;
        //print("Turned on");
    }
    public void TurnOff() {
        isOn = false;
        //("Turned off");
    }
}
