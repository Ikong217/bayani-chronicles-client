using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(EmoteEventVars))]
public class EmoteEditor : PropertyDrawer
{
    public VisualTreeAsset VisualTree;
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new VisualElement();
        if (VisualTree == null)
        {
            Debug.LogError("VisualTreeAsset is not assigned!");
            return root;
        }
        VisualTree.CloneTree(root);

        var playerBool = root.Q<PropertyField>("player");
        var charContainer = root.Q<VisualElement>("CharacterContainer");

        var playerProperty = property.FindPropertyRelative("player");

        UpdateCharacterDisplay(charContainer, playerProperty.boolValue);

        playerBool.RegisterValueChangeCallback(evt =>
        {
            UpdateCharacterDisplay(charContainer, playerProperty.boolValue);
        });

        return root;
    }

    private void UpdateCharacterDisplay(VisualElement character, bool player)
    {
        if (player)
            character.style.display = DisplayStyle.None;
        else
            character.style.display = DisplayStyle.Flex;
    }
}
