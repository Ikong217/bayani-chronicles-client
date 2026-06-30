using UnityEngine;

// Handles triggering scroll collection
public class ScrollTrigger : MonoBehaviour
{
    public string imageFilePath;
    private ScrollCounter scrollCounter;
    private AudioClip scrollSound;
    private AudioSource audioSource;

    //[Header("Resources/Scrolls/...  You can add subfolder here from the image location")]
    //public string imageSourceDirectory = "";
    public bool enabledObj = true;
    public bool recordScroll = true;
    private bool isRegistered = false;

    private void Awake()
    {
        isRegistered = false;

        GameObject scrollCounterObj = GameObject.Find("ScrollCounter");
        scrollSound = Resources.Load<AudioClip>("Scroll_collect");
        audioSource = GetComponent<AudioSource>();

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

    private void Start()
    {
        RegisterAndOff();
    }

    public void RegisterAndOff()
    {
        if (!isRegistered)
        {
            if (scrollCounter != null)
            {
                scrollCounter.RegisterScroll();
            }
            //ScrollInventoryContainer.LoadData().ClearData();
            gameObject.SetActive(enabledObj);
            isRegistered = true;
        }
        else
        {
            print("Already Registered");
        }
    }

    public void SaveData()
    {
        ScrollInventoryContainer scrollContainer = ScrollInventoryContainer.LoadData();

        ScrollShowTrigger trigger = GetComponent<ScrollShowTrigger>();
        ScrollContent line = trigger.scrollContent;

        string object_name = null;
        if (recordScroll)
        {
            QuestionsRequestHandler requestHandler = QuestionsRequestHandler.Instance;
            string novel_name = EnumHelper.GetNovel(requestHandler.novel);
            string level_name = EnumHelper.GetLevel(requestHandler.level);

            object_name = novel_name + "/" + level_name + "/" + gameObject.name;
        }

        // Save using sprite name
        if (recordScroll)
        {
            scrollContainer.AddScrollItem(object_name, imageFilePath, line.title, line.sprite, line.imageDescription, line.description);

            if (!scrollContainer.isSaved)
            {
                scrollContainer.SaveData();
                ProgressData.Altered();
            }
        }

        audioSource.PlayOneShot(scrollSound, 0.6f);
    }
    private void OnDestroy()
    {
        scrollCounter.TriggeredScroll();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActiveAndEnabled)
            return;

        if (collision.CompareTag("Offset") && scrollCounter != null)
        {
            SaveData();
        }
    }
}
