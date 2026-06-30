using UnityEngine;

public class AutoTeleporterScript : MonoBehaviour
{
    public GameObject teleportOjbect;
    public bool isPlayer = false;
    private TeleporterScript tpScript;
    private void Start()
    {
        tpScript = gameObject.GetComponent<TeleporterScript>();
        if (tpScript == null)
            Debug.LogError("Missing Component TeleportScript: " + gameObject.name);
    }

    private void OnEnable()
    {
        tpScript = gameObject.GetComponent<TeleporterScript>();
        if (tpScript == null)
            Debug.LogError("Missing Component TeleportScript: " + gameObject.name);
        //print(isPlayer);
        if (isPlayer)
        {
            teleportOjbect = Player.FindPlayer();
            print(teleportOjbect);
        }

        tpScript.Teleport(teleportOjbect);

    }
}
