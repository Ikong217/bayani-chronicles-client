using UnityEngine;

[System.Serializable]
public class Character : MonoBehaviour
{
    private string _name = "NO name";
    private Sprite _sprite;
    private Gender _gender = Gender.MALE;
    private Color _myColor = Color.white;
    private void Start()
    {
        //print(this._gender);
        //_charEmote.Emote(_initialEmote);
    }

    private void Awake()
    {
    }

    public void SaveInfo(string name = "", Sprite sprite = null, Gender gender = Gender.Null, Color ? myColor = null)
    {
        if (name != "")
        {
            this._name = name;
        }

        if (sprite != null)
        {
            this._sprite = sprite;
        }

        if (gender != Gender.Null)
        {
            this._gender = gender;
        }
        if (myColor.HasValue)
        {
            _myColor = myColor.Value;
        }
    }

    public void SetSrite(Sprite sprite)
    {
        _sprite = sprite;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public string GetName()
    {
        return _name;
    }

    public Gender GetGender()
    {
        return _gender;
    }

    public Sprite GetSprite()
    {
        return _sprite;
    }

    public Color GetColor()
    {
        return _myColor;
    }

}

[System.Serializable]
public class GenderWrapper
{
    public Gender selectedGender = Gender.MALE;
}

public enum Gender
{
    MALE,
    FEMALE,
    Null
}