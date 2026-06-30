#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GenderWrapper))]
public class GenderWrapperDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var genderProp = property.FindPropertyRelative("selectedGender");

        EditorGUI.BeginProperty(position, label, property);

        // Create a filtered enum without "Null"
        var genders = System.Enum.GetValues(typeof(Gender));
        var displayNames = System.Enum.GetNames(typeof(Gender));
        int[] filteredIndices = new int[] { 0, 1 }; // Assuming MALE = 0, FEMALE = 1

        string[] filteredNames = { "MALE", "FEMALE" };
        int selectedIndex = genderProp.enumValueIndex;
        if (selectedIndex >= filteredNames.Length) selectedIndex = 0;

        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, filteredNames);
        genderProp.enumValueIndex = filteredIndices[newIndex];

        EditorGUI.EndProperty();
    }
}
#endif