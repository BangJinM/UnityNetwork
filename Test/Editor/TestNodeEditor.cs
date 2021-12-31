using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestNode))]
class TestNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Click Me"))
        {
            //SerializedProperty graphProp = serializedObject.FindProperty("graph");
            //TestNodeWindow w = TestNodeWindow.Open(graphProp.objectReferenceValue as XNode.NodeGraph);
            //w.Home(); // Focus selected node
            TestNodeWindow w = TestNodeWindow.OpenWindow();
        }
    }
}
