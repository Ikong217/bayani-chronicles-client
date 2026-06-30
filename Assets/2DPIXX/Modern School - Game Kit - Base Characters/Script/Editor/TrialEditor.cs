using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;

[CustomEditor(typeof(TrialTriggerScript))]
public class TrialEditor : Editor
{
    public VisualTreeAsset VisualTree;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        VisualTree.CloneTree(root);

        return root;
    }
}
