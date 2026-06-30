using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
public class ScoringScript : MonoBehaviour
{
    public GameObject dialogueBox;
    public GameObject itemDialogue;
    public TextMeshProUGUI text;
    public Image image;
    public GameObject trigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogueBox.SetActive(true);
            itemDialogue.SetActive(true);
            image.enabled = false;
            text.text = "Your final score is " + PlayerPrefs.GetInt("PlayerScore");
            StartCoroutine(WaitToDie());
        }
    }

    IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(3f);
        image.enabled = true;
        itemDialogue.SetActive(false);
        dialogueBox.SetActive(false);
        Destroy(trigger);
    }
}
