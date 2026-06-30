using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(DialogueLine))]
public class DialogueEditor : PropertyDrawer
{
    public VisualTreeAsset VisualTree;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create the root element
        var root = new VisualElement();

        // Load and clone the visual tree
        if (VisualTree == null)
        {
            Debug.LogError("VisualTreeAsset is not assigned!");
            return root;
        }
        VisualTree.CloneTree(root);


        var playerBool = root.Q<PropertyField>("isPlayer");
        var characterContainer = root.Q<VisualElement>("CharacterContainer");

        var playerProperty = property.FindPropertyRelative("player");


        var dialogueEnum = root.Q<EnumField>("DialogueType");
        var characterArea= root.Q<VisualElement>("CharacterArea");
        var itemContainer = root.Q<VisualElement>("Item");

        var dialogueTypeProperty = property.FindPropertyRelative("dialogueType");


        UpdateCharacterDisplay(characterContainer, playerProperty.boolValue);

        playerBool.RegisterValueChangeCallback(evt =>
        {
            UpdateCharacterDisplay(characterContainer, playerProperty.boolValue);
        });

        if (dialogueEnum != null)
        {
            dialogueEnum.Init((DialogueType)dialogueTypeProperty.enumValueIndex);
            dialogueEnum.BindProperty(dialogueTypeProperty);
        }

        UpdateTypeDisplay(characterArea, itemContainer, (DialogueType)dialogueTypeProperty.enumValueIndex);

        if (dialogueEnum != null)
        {
            dialogueEnum.RegisterValueChangedCallback(evt =>
            {
                UpdateTypeDisplay(characterArea, itemContainer, (DialogueType)evt.newValue);
            });
        }


        return root;
    }

    private void UpdateCharacterDisplay(VisualElement character, bool player)
    {
        if (player)
        {
            character.style.display = DisplayStyle.None;
        }
        else
        {
            character.style.display = DisplayStyle.Flex;
        }
    }

    private void UpdateTypeDisplay(VisualElement character, VisualElement item, DialogueType dialogueType)
    {
        //Debug.Log("pressed");
        if (character == null || item == null) return;

        bool isCharacterDialogue = dialogueType == DialogueType.character;
        character.style.display = isCharacterDialogue ? DisplayStyle.Flex : DisplayStyle.None;
        item.style.display = isCharacterDialogue ? DisplayStyle.None : DisplayStyle.Flex;
    }


}

public enum DialogueType
{
    character,
    item
}