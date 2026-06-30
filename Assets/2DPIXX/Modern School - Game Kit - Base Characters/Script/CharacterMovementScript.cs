using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    private CharAnimation charAnim;
    private bool walking = false;
    public float CharacterSpeed = 0f;
    public DirectionType direction = DirectionType.BACK;
    private void Start()
    {
        charAnim = GetComponent<CharAnimation>();
    }

    public void AddSpeed()
    {
        CharacterSpeed = 1f;
        walking = true;
    }

    public void RemoveSpeed()
    {
        CharacterSpeed = 0f;
        walking = false;
    }

    public void SetDirection(DirectionType directionType)
    {
        direction = directionType;
    }

    public void Update()
    {
        if (charAnim)
        {
            charAnim.SetDirection(direction);
            charAnim.Walking(walking);
        }
        
    }
}
