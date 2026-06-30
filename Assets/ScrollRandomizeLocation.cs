using UnityEngine;

public class ScrollRandomizeLocation : MonoBehaviour
{
    private ScrollShowTrigger sctrig ;
    private ScrollTrigger strig;
    private CharacterMovement player;
    private float fs;
    [SerializeField] private RandomLocation locs;

    private void Start()
    {
        sctrig = gameObject.GetComponent<ScrollShowTrigger>();
        strig = gameObject.GetComponent<ScrollTrigger>();

        GameObject p = Player.FindPlayer();
        player = p.GetComponent<CharacterMovement>();



        if (sctrig == null)
            Debug.LogWarning("No available showscrolltrigger");
        else
            sctrig.enabled = false;

        if (strig == null)
        {
            Debug.LogWarning("No available DialogueTrigger");
        }
        else
        {
            strig.RegisterAndOff();
            strig.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print("Dtrig" + dtrig.isActiveAndEnabled);
        //print("Strig" + strig.isActiveAndEnabled);

        if (collision.CompareTag("Offset") && sctrig != null)
        {
           // print(fs >= 0.7f);
            if(fs >= 0.7f)
            {
                //print("Teleported");
                transform.position = locs.GetRandomLocation().position;
            }
            else
            {
                //print("Triggered");
                sctrig.enabled = true;
                sctrig.PubTrigger();
                strig.enabled = true;
                strig.SaveData();
            }
        }
    }
    private void Update()
    {
        fs = player.GetForceSpeed();
    }
}
