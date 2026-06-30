using UnityEngine;
using System.Collections;

[System.Serializable]
public enum DirectionType
{
    RIGHT = 1,
    BACK = 2,
    LEFT = 3,
    FRONT = 4
}

[System.Serializable]
public class CharAnimation : MonoBehaviour
{
    public DirectionType initialDirection = DirectionType.FRONT;
    public float soundRange = 10f;
    public AudioSource footstepSource;

    private DirectionType lastDirection;
    private Vector2 oldPosition;
    private bool moving = false;
    private bool manualAnimation = false;

    private Animator anim;

    // Animator parameter hashes
    private static readonly int PosXHash = Animator.StringToHash("PosX");
    private static readonly int PosYHash = Animator.StringToHash("PosY");
    private static readonly int WalkHash = Animator.StringToHash("Walk");

    private static readonly DirectionConverter directionConverter = new DirectionConverter();

    void Start()
    {
        anim = GetComponent<Animator>();

        if (!anim)
        {
            Debug.LogError("Missing component: Animator on {" + gameObject.name + "}");
            return;
        }

        oldPosition = transform.position;
        moving = false;
        SetDirection(initialDirection);
        lastDirection = initialDirection;

        if (footstepSource == null)
        {
            // Load prefab from Resources
            GameObject prefab = Resources.Load<GameObject>("SoundsTracker");

            if (prefab != null)
            {
                // Instantiate at this object's position
                GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);

                // Optional: parent it to this GameObject
                instance.transform.SetParent(transform);

                // Start coroutine to assign the AudioSource
                StartCoroutine(GetFootstep(instance));
            }
            else
            {
                Debug.LogError("SoundsTracker prefab not found in Resources!");
            }
        }
    }
    public DirectionType GetCurrentDirection() => lastDirection;

    public void SetAnimSpeed(float speed)
    {
        anim.speed = speed;
    }

    private IEnumerator GetFootstep(GameObject footstepObj)
    {
        yield return null; // skip one frame to ensure Instantiate completes
        footstepSource = footstepObj.GetComponent<AudioSource>();

        if (footstepSource == null)
            Debug.LogError("No AudioSource found on SoundsTracker prefab!");



        // Match Z to camera so it's not "far" in 2D
        Vector3 pos = footstepSource.transform.position;
        pos.z = Camera.main.transform.position.z;
        footstepSource.transform.position = pos;
    }


    public void SetDirection(DirectionType direction)
    {
        //print(direction);
        if (lastDirection != direction)
        {
            anim.SetFloat(PosXHash, directionConverter.GetPosX(direction));
            anim.SetFloat(PosYHash, directionConverter.GetPosY(direction));
            lastDirection = direction;
        }
    }

    public void Walking(bool walk)
    {
        if (moving != walk)
        {
            anim.SetBool(WalkHash, walk);
            moving = walk;
        }

        SetSoundFootstep(walk);
    }

    public bool IsWalking() => moving;

    public void SetSoundFootstep(bool active)
    {
        if (footstepSource != null)
        {
            // Configure for 3D sound with range limit
            footstepSource.spatialBlend = 1f; // 3D sound
            footstepSource.minDistance = 1f;
            footstepSource.maxDistance = soundRange;
            footstepSource.rolloffMode = AudioRolloffMode.Linear;

            if (active)
            {
                if (!footstepSource.isPlaying)
                    footstepSource.Play();
            }
            else
            {
                if (footstepSource.isPlaying)
                    footstepSource.Pause();
            }
        }
    }



    void FixedUpdate()
    {
        if(!manualAnimation)
            AnimateMoving();
    }

    public void ManualAnimate(bool isManual)
    {
        manualAnimation = isManual;
    }

    private void AnimateMoving()
    {
        Vector2 currentPosition = transform.position;
        Vector2 delta = currentPosition - oldPosition;

        if (delta.sqrMagnitude > 0.0001f) // moving
        {
            Walking(true);
            SetDirection(DirectionHelper.GetDirection(delta));
            oldPosition = currentPosition;
        }
        else // idle
        {
            Walking(false);
        }
    }


    public DirectionType GetInitialDirection()
    {
        return initialDirection;
    }
}

public class DirectionConverter
{
    public float GetPosX(DirectionType dir)
    {
        return dir switch
        {
            DirectionType.RIGHT => 1f,
            DirectionType.LEFT => -1f,
            _ => 0f,
        };
    }

    public float GetPosY(DirectionType dir)
    {
        return dir switch
        {
            DirectionType.BACK => 1f,
            DirectionType.FRONT => -1f,
            _ => 0f,
        };
    }

    public Vector2 GetVector2(DirectionType directionType)
    {
        return new Vector2(GetPosX(directionType), GetPosY(directionType));
    }
}

public static class DirectionHelper
{
    public static DirectionType GetDirection(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        if (angle >= 45 && angle < 135)
            return DirectionType.BACK;
        else if (angle >= 135 && angle < 225)
            return DirectionType.LEFT;
        else if (angle >= 225 && angle < 315)
            return DirectionType.FRONT;
        else
            return DirectionType.RIGHT;
    }
}
