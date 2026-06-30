using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(SetLocationCharacter))]
public class SetLocationEditor : PropertyDrawer
{
    public VisualTreeAsset VisualTree;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create the root element
        VisualElement root = new VisualElement();

        // Clone the visual tree
        VisualTree.CloneTree(root);

        // Get references to the elements
        var isPlayerField = root.Q<PropertyField>("isPlayer");
        var characterShowField = root.Q<VisualElement>("CharacterShowField");

        // Get the serialized property
        var showValuesProperty = property.FindPropertyRelative("player");

        var isWalk = root.Q<PropertyField>("WalkBool");
        var locationContainer = root.Q<VisualElement>("Locationcontainer");
        var directionContainer = root.Q<VisualElement>("DirectionContainer");

        var walkingProperty = property.FindPropertyRelative("walk");

        // Set initial state
        UpdateDisplay(characterShowField, showValuesProperty.boolValue);
        UpdateActionDisplay(locationContainer, directionContainer, walkingProperty.boolValue); ;

        // Register callback for changes
        isPlayerField.RegisterValueChangeCallback(evt =>
        {
            UpdateDisplay(characterShowField, showValuesProperty.boolValue);
        });

        isWalk.RegisterValueChangeCallback(evt =>
        {
            UpdateActionDisplay(locationContainer, directionContainer, walkingProperty.boolValue);;
        });

        return root;
    }

    private void UpdateDisplay(VisualElement characterShowField, bool showValues)
    {
        if (showValues)
        {
            characterShowField.style.display = DisplayStyle.None;
        }
        else
        {
            characterShowField.style.display = DisplayStyle.Flex;
        }
    }

    private void UpdateActionDisplay(VisualElement location, VisualElement direction, bool walk)
    {
        if (walk)
        {
            location.style.display = DisplayStyle.Flex;
            direction.style.display = DisplayStyle.None;
        }
        else
        {
            location.style.display = DisplayStyle.None;
            direction.style.display = DisplayStyle.Flex;
        }
    }
}