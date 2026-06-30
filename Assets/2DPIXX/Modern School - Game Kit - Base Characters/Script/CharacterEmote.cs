using UnityEngine;
using System.Collections;

public class CharacterEmote : MonoBehaviour
{
    private GameObject bubbles;
    private Transform bubblePos;
    private EmoticonScript emoticon;
    public Emotes emotion;
    public float Duration = 5.0f;

    private void Start()
    {
        bubblePos = transform;
        bubbles = Resources.Load<GameObject>("Emoticon");

        Emote(emotion,Duration);

    }
    public void Emote(Emotes emotes, float limit = 5f)
    {
        if(emotes != Emotes.none)
        {
            //print("nothing beats the jet 2 hollidays");
            GameObject bubbleInstance = Instantiate(bubbles, bubblePos.position, bubblePos.rotation, bubblePos);
            StartCoroutine(PlayAfterStart(bubbleInstance, emotes));
            if(limit > 0)
                StartCoroutine(Limit(limit));
        }
    }

    public void EndEmote()
    {
        if (emoticon)
        {
            emoticon.Kill();
            emoticon = null;
        }
        else
        {
            print("no emoticon present");
        }
    }
    IEnumerator PlayAfterStart(GameObject bubbleInstance, Emotes emotes)
    {
        yield return null; // wait 1 frame
        emoticon = bubbleInstance.GetComponent<EmoticonScript>();
        emoticon.ShowSprite();
        bubbleInstance.transform.position = new Vector2(bubblePos.position.x, bubblePos.position.y + 0.725f);
        //yield return null;
        if (emoticon != null)
        {
            emoticon.SetAnimation(emotes);
        }
    }

    IEnumerator Limit(float limit)
    {
        yield return new WaitForSeconds(limit);
        EndEmote();
    }

    public Emotes GetEmote() => emotion;
}
