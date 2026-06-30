using UnityEngine;
using System.Collections;
using TMPro;

public class ScrollCounter : MonoBehaviour
{
    private int scrollcount = 0;
    private int maxScroll = 0;
    public TextMeshProUGUI text;
    public GameObject questionHandler;
    public bool scrollRegistered;
    private bool activated = false;
    //public GameObject player;
    //public Transform location;
    //public GameObject triggerObjext;
    //public int maxScroll = 9;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        scrollcount = 0;
        maxScroll = 0;
        text.text = "00/00";
    }
    private void Start()
    {
        scrollRegistered = false; 
        StartCoroutine(WaitLoadScrolls(2));
    }
    IEnumerator WaitLoadScrolls(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        scrollRegistered = true;
    }

    private void Update()
    {
        if(scrollRegistered && (maxScroll == scrollcount))
        {
            StartCoroutine(WaitLastFinish());
        }
    }
    IEnumerator WaitLastFinish()
    {
        //print("natp ndiri");
        yield return new WaitUntil(() => !Player.FindPlayer().GetComponent<CharacterMovement>().onGoingEvent);

        TeleporterScript teleporter = gameObject.GetComponent<TeleporterScript>();
        teleporter.Teleport(Player.FindPlayer());
        if (!activated)
        {
            questionHandler.SetActive(true);
            activated = true;
        }
    }

    public void RegisterScroll()
    {
        maxScroll += 1;
        UpdateScrollCount();
    }

    public void TriggeredScroll()
    {
        scrollcount += 1;
        UpdateScrollCount();
    }

    private void UpdateScrollCount()
    {
        text.text = scrollcount.ToString() + "/" + maxScroll.ToString();
    }

    public int GetScrollsCount() => scrollcount;
    //public void Update()
    //{
    //    if (this.gameObject.activeSelf)
    //    {
    //        scrollcount += 1;
    //        this.gameObject.SetActive(false);
    //    }
    //
    //    if(scrollcount >= maxScroll)
    //    {
    //        player.transform.position = location.position;
    //        //CharacterController playerAnim = player.GetComponent<CharacterController>();
    //        //playerAnim.RemoveSpeed();
    //        //playerAnim.SetDirection(DirectionType.BACK);
    //        triggerObjext.SetActive(true);
    //        Destroy(this.gameObject);
    //    }
    //}
}
