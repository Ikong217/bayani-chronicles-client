using UnityEngine;

public enum Emotes
{
    none,       // 0
    shy,        // 1
    heart,      // 2
    attention,  // 3
    angry,      // 4
    silence,    // 5
    question,   // 6
    sweat,      // 7
    idea,       // 8
    sad,        // 9
    sleep       // 10
}

public class EmoticonScript : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sprite;

    private void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        HideSprite();
    }

    public void ShowSprite()
    {
        sprite.enabled = true;
    }

    public void HideSprite()
    {
        sprite.enabled = false;
    }

    public void SetAnimation(Emotes emote)
    {
        if (emote != Emotes.none)
        {
            float emoteValue = ConvertEmote(emote);
            anim.SetFloat("Emote", emoteValue);
            //Debug.Log($"Set Emote: {emote} -> {emoteValue}");
        }
    }

    private float ConvertEmote(Emotes emote)
    {
        float scale = 1f / 9f;
        return ((int)emote - 1) * scale;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
