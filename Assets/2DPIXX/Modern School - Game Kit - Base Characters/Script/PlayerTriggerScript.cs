using UnityEngine;

public class PlayerTriggerScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("npc"))
        {
            CharacterEmote characterEmote = other.GetComponent<CharacterEmote>();
            if (characterEmote != null)
            {
                //print("asdhf");
                //  teacher.Shy();
                characterEmote.EndEmote();
            }
        }
    }
}
