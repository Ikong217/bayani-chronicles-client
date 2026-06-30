using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(Events))]
public class EventsEditor : PropertyDrawer
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

        var eventTypeEnum = root.Q<EnumField>("EventEnum");
        var dialogueCont = root.Q<VisualElement>("DialogueContainer");
        var setLocCont = root.Q<VisualElement>("SetLocationContainer");
        var emoteCont = root.Q<VisualElement>("EmoteContainer");
        var qCont = root.Q<VisualElement>("QuestionContainer");

        var eventTypeProp = property.FindPropertyRelative("eventType");

        if (eventTypeEnum != null)
        {
            eventTypeEnum.Init((EventType)eventTypeProp.enumValueIndex);
            eventTypeEnum.BindProperty(eventTypeProp);
        }

        UpdateEventEnumDisplay(dialogueCont, setLocCont, emoteCont, qCont, (EventType)eventTypeProp.enumValueIndex);
        eventTypeEnum.RegisterValueChangedCallback(evt =>
        {
            UpdateEventEnumDisplay(dialogueCont, setLocCont, emoteCont, qCont, (EventType)evt.newValue);
        });

        return root;
    }

    public void UpdateEventEnumDisplay(VisualElement dialogue, VisualElement setLoc, VisualElement emote, VisualElement question, EventType eventType)
    {
        if (dialogue == null || setLoc == null) return;

        if(eventType == EventType.dialogue) 
        { 

            dialogue.style.display = DisplayStyle.Flex;
            setLoc.style.display = DisplayStyle.None;
            emote.style.display = DisplayStyle.None;
            question.style.display = DisplayStyle.None;
        }
        else if(eventType == EventType.setLocation)
        {
            setLoc.style.display = DisplayStyle.Flex;
            dialogue.style.display = DisplayStyle.None;
            emote.style.display = DisplayStyle.None;
            question.style.display = DisplayStyle.None;
        }
        else if (eventType == EventType.setEmotes)
        {
            setLoc.style.display = DisplayStyle.None;
            dialogue.style.display = DisplayStyle.None;
            emote.style.display = DisplayStyle.Flex;
            question.style.display = DisplayStyle.None;
        }
        else if (eventType == EventType.questions)
        {
            setLoc.style.display = DisplayStyle.None;
            dialogue.style.display = DisplayStyle.None;
            emote.style.display = DisplayStyle.None;
            question.style.display = DisplayStyle.Flex;
        }
        else
        {
            dialogue.style.display = DisplayStyle.Flex;
            setLoc.style.display = DisplayStyle.None;
        }
    }

}



public enum EventType
{
    dialogue,
    setLocation,
    setEmotes,
    questions
}