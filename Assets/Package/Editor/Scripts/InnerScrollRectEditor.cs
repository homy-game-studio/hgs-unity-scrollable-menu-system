using UnityEditor;

namespace HGS.ScrollableMenuSystem.Editor
{
    [CustomEditor(typeof(InnerScrollRect), true)]
    public class InnerScrollRectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}