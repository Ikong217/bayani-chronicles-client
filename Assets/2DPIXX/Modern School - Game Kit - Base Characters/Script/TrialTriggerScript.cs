using UnityEngine;

public class TrialTriggerScript : MonoBehaviour
{
    public bool player = true;
    public GameObject character;
    //AnimationScript anim;
    public Transform tartgetLocation;
    public DirectionType direction;
    public bool walk;
    public float cooldown = 0f;
}
