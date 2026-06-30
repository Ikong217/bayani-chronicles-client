using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmoteManager : MonoBehaviour
{
    public static EmoteManager Instance;
    private GameObject character;
    public bool emoteFinished = false;

    private Queue<EmoteEventVars> emoteEventVars = new Queue<EmoteEventVars>();
    private bool subEvent = false;
    private MiscManager miscManager;
    private Camera cam;
    private CamFollow camscr;

    void Awake()
    {
        
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        miscManager = gameObject.GetComponent<MiscManager>();
        cam = GameObject.FindFirstObjectByType<Camera>();
        camscr = cam.GetComponent<CamFollow>();
    }

    public void StartEmote(SetCharacterEmotes setCharacterEmotes)
    {
        emoteFinished = false;
        emoteEventVars.Clear();

        foreach(EmoteEventVars emotevars in setCharacterEmotes.emotions)
        {
            emoteEventVars.Enqueue(emotevars);
        }

        if(emoteEventVars.Count < 1)
        {
            EndEmote();
        }

        if (miscManager.isRunning())
        {
            subEvent = true;
        }
        else
        {
            subEvent = false;
            miscManager.StartEvent();
        }
        HandleEmoteActions();
    }

    private void HandleEmoteActions()
    {
        while(emoteEventVars.Count > 0)
        {
            EmoteEventVars emoteEvents = emoteEventVars.Dequeue();

            if (emoteEvents.player)
            {
                character = Player.FindPlayer();
            }
            else
            {
                character = emoteEvents.character;
            }
            //print(emoteEvents.emotes);

            CharacterEmote charEmote = character.GetComponent<CharacterEmote>();
            //charEmote.SeeActiveGameobject();
            if (emoteEvents.emotes == Emotes.none)
            {
                charEmote.EndEmote();
            }
            else
            {
                charEmote.Emote(emoteEvents.emotes, emoteEvents.duration);
            }
        }
        EndEmote();
    }

    


    public void EndEmote()
    {
        if (subEvent)
        {
            subEvent = false;
        }
        else
        {
            miscManager.EndEvent();
        }
        emoteFinished = true;
    }

}
