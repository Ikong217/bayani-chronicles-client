using UnityEngine;

public class Student : Character
{
    public string _student_name;
    public Sprite _student_Sprite;
    public GenderWrapper _student_gender = new GenderWrapper();

    private void Awake()
    {
        Gender gender = _student_gender.selectedGender;
        SaveInfo(_student_name, _student_Sprite, gender);
    }
}
