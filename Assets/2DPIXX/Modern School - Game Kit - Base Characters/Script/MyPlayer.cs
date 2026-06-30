using UnityEngine;

public class MyPlayer : Character
{
    public Sprite _player_Sprite;
    private string _player_name;
    private Gender _player_gender = Gender.MALE;
    private Color mycolor = Color.green;

    // Player minimap icon
    private GameObject playerNavigation;
    private CharAnimation anim;

    private void Awake()
    {
        ConvertData();
        SaveInfo(_player_name, _player_Sprite, _player_gender, mycolor);

        // Load and instantiate the minimap icon at this GameObject's position
        GameObject prefab = Resources.Load<GameObject>("PlayerNavigation");
        if (prefab != null)
        {
            // Instantiate at the current GameObject's position and rotation
            playerNavigation = Instantiate(prefab, transform.position, transform.rotation, transform);

            //return the actual prefab sixe
            prefab.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
        }
        else
        {
            Debug.LogError("PlayerNavigation prefab not found in Resources folder!");
        }

        // Get the CharAnimation component
        anim = GetComponent<CharAnimation>();
        if (anim == null)
        {
            Debug.LogError("CharAnimation component not found on player!");
        }
    }

    private void Update()
    {
        if (playerNavigation == null || anim == null)
            return;

        // Rotate the minimap icon instantly based on direction
        DirectionType direction = anim.GetCurrentDirection();
        playerNavigation.transform.rotation = DirectionConvert(direction);
    }

    private void ConvertData()
    {
        _player_name = PlayerPrefs.GetString("username");
        string gender = PlayerPrefs.GetString("gender").ToLower();

        _player_gender = gender == "female" ? Gender.FEMALE : Gender.MALE;
    }

    private Quaternion DirectionConvert(DirectionType direction)
    {
        float angle = 0f;

        switch (direction)
        {
            case DirectionType.BACK:
                angle = 0f;      // Facing up (north)
                break;
            case DirectionType.LEFT:
                angle = 90f;     // Counter-clockwise
                break;
            case DirectionType.FRONT:
                angle = 180f;    // Facing down (south)
                break;
            case DirectionType.RIGHT:
                angle = -90f;    // Clockwise
                break;
        }

        // Return a rotation for 2D space (Z-axis)
        return Quaternion.Euler(0f, 0f, angle);
    }
}
