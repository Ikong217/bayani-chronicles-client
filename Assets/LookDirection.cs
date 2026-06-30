using UnityEngine;

public class LookDirection : MonoBehaviour
{
    private DirectionType initialDirection;
    private CharAnimation anim;
    private CharacterEmote emote;
    private Emotes InitialEmote;
    public Emotes noticeEmote = Emotes.none;

    private void Start()
    {
        anim = GetComponent<CharAnimation>();
        if (anim == null)
        {
            // Remove only this script component, not the GameObject
            Destroy(this);
        }
        emote = gameObject.GetComponent<CharacterEmote>();
    }

    public void SetEmote(Emotes emotes)
    {
        if(emote != null)
        {
            if(emotes == Emotes.none)
            {
                if(InitialEmote == Emotes.none)
                {
                    emote.EndEmote();
                }
                else
                {
                    emote.Emote(InitialEmote);
                }
            }
            else
            {
                emote.Emote(emotes);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Offset"))
        {
            // Store current direction before facing target
            initialDirection = anim.GetCurrentDirection();
            InitialEmote = emote.GetEmote();
            SetEmote(noticeEmote);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Offset"))
        {
            // Get direction vector to the object we touched
            Vector2 directionToTarget = Player.FindPlayer().transform.position - transform.position;

            // Determine the facing direction using your helper
            DirectionType lookDirection = DirectionHelper.GetDirection(directionToTarget);

            // Update animation to face that direction
            anim.SetDirection(lookDirection);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Offset"))
        {
            // Restore the original direction
            anim.SetDirection(initialDirection);
            SetEmote(Emotes.none);
        }
    }
}
