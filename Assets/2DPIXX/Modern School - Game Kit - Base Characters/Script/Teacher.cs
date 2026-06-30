using UnityEngine;

public class Teacher : Character
{
    public string _teacher_name;
    public Sprite _teacher_Sprite;
    public GenderWrapper _teacher_gender = new GenderWrapper();

    private void Awake()
    {
        Gender gender = _teacher_gender.selectedGender;
        SaveInfo(_teacher_name, _teacher_Sprite, gender);
    }
}
