using UnityEngine;
using System.Collections;

public class SpawnUsername : MonoBehaviour
{
    private GameObject usernameObj;
    private Transform offSet;
    public float extension = 0f;

    private void Start()
    {
        usernameObj = Resources.Load<GameObject>("Username");
        offSet = transform;
        Spawn();
    }

    private void Spawn()
    {
        GameObject usernameInstance = Instantiate(usernameObj, offSet.position, offSet.rotation, offSet);

        // Reset scale so it won’t inherit parent’s scale
        usernameInstance.transform.localScale = new Vector3(1 / gameObject.transform.localScale.x, 1 / gameObject.transform.localScale.y, gameObject.transform.localScale.x);

        // finding the character
        Character character = null;

        MyPlayer myPlayer = GetComponent<MyPlayer>();
        Teacher teacher = GetComponent<Teacher>();
        Student student = GetComponent<Student>();

        if (myPlayer != null)
            character = myPlayer as Character;
        else if (teacher != null)
            character = teacher as Character;
        else if (student != null)
            character = student as Character;
        else
            character = new Character();

        StartCoroutine(PlayAfterStart(usernameInstance, character));
    }

    IEnumerator PlayAfterStart(GameObject usernameInstance, Character character)
    {
        yield return null;

        TextMesh text = usernameInstance.GetComponent<TextMesh>();
        Renderer textRend = usernameInstance.GetComponent<Renderer>();

        usernameInstance.transform.position = new Vector2(offSet.position.x, offSet.position.y + 1f + extension);

        if (text == null)
            Debug.LogError("Missing component TextMesh");
        else
        {
            text.text = character.GetName();
            text.color = character.GetColor();
        }

        if (textRend == null)
            Debug.LogError("Missing component Renderer: " + text.gameObject);
        else
        {
            textRend.sortingLayerName = "Default";
            textRend.sortingOrder = 100;
            textRend.sortingLayerName = "Player";
        }
    }
}
