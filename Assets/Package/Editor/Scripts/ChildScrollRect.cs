using UnityEditor;

namespace HGS.ScrollableMenuSystem.EditorExtensions
{
  [CustomEditor(typeof(ChildScrollRect), true)]
  public class ChildScrollRectEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      serializedObject.ApplyModifiedProperties();
    }
  }
}