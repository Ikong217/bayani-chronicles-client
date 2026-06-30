using UnityEngine;

public class NPCWander2D : MonoBehaviour
{
    [Header("Roaming Area")]
    public BoxCollider2D area;  // Assign the collider in Inspector

    [Header("Movement Settings")]
    public float speed = 2f;
    public float waitTime = 2f;

    [Header("Extra")]
    public float align = 0.6f; // Visual Y offset

    private Vector2 targetPosition;
    private float waitTimer;
    private Vector2 basePosition; // Actual movement position (without visual offset)

    private bool isInteracted = false;
    private CharAnimation anim;
    private CharacterMovement playerMovement;
    private bool destinationPicked = false;

    void Start()
    {
        basePosition = transform.position;
        anim = gameObject.GetComponent<CharAnimation>();
        playerMovement = Player.FindPlayer().GetComponent<CharacterMovement>();
        PickNewDestination();
    }

    void Update()
    {
        if(playerMovement != null)
        {
            if (playerMovement.onGoingEvent)
            {
                if (destinationPicked)
                {
                    //print("nagana to");
                    destinationPicked = false;
                }
                basePosition = new Vector3(transform.position.x, transform.position.y - align, transform.position.z);
                return;
            }
        }

        if (!destinationPicked)
        {
            PickNewDestination();
            return;
        }

        if (!isInteracted)
        {
            // Move the base position toward the target (no offset)
            basePosition = Vector2.MoveTowards(basePosition, targetPosition, speed * Time.deltaTime);

            // Apply visual offset (this moves sprite only)
            transform.position = new Vector3(basePosition.x, basePosition.y + align, transform.position.z);

            //animate walking
            anim.Walking(true);

            // Check if reached destination
            if (Vector2.Distance(basePosition, targetPosition) < 0.1f)
            {
                waitTimer += Time.deltaTime;
                anim.Walking(false);
                if (waitTimer >= waitTime)
                {
                    PickNewDestination();
                    waitTimer = 0f;
                }
            }
        }
        else
        {
            anim.Walking(false);
        }
    }

    void PickNewDestination()
    {
        if (area == null) return;
        destinationPicked = true;
        // Get box bounds (in world space)
        Vector2 center = (Vector2)area.transform.position + area.offset;
        Vector2 size = area.size;

        // Pick a random point inside the 2D box
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);

        targetPosition = new Vector2(x, y);
    }

    private void OnDrawGizmosSelected()
    {
        if (area != null)
        {
            Gizmos.color = Color.green;
            Vector2 center = (Vector2)area.transform.position + area.offset;
            Gizmos.DrawWireCube(center, area.size);
        }
    }

    // ✅ Now using Trigger instead of Collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Offset"))
        {
            isInteracted = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Offset"))
        {
            isInteracted = false;
        }
    }

    public void TurnOff() => isInteracted = true;
    public void TurnOn() => isInteracted = false;
    public void ToggleActive() => isInteracted = !isInteracted;
}
