using UnityEditor;
using UnityEditor.Callbacks;

class TestNodeWindow : EditorWindow
{
    #region 打开界面

    [OnOpenAsset(0)]
    public static bool OpenWindow(int instanceID, int line)
    {
        TestNode nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as TestNode;
        if (nodeGraph != null)
        {
            OpenWindow();
            return true;
        }
        return false;
    }
    public static TestNodeWindow OpenWindow()
    {
        TestNodeWindow w = GetWindow(typeof(TestNodeWindow), false, "TestNode", true) as TestNodeWindow;
        w.wantsMouseMove = true;
        return w;
    }
    #endregion

    protected virtual void OnGUI()
    {
      
    }
}
