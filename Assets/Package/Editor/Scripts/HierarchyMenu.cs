using UnityEditor;
using UnityEngine;

namespace HGS.ScrollableMenuSystem.EditorExtensions
{
  public class HierarchyMenu
  {
    [MenuItem("GameObject/HGS/ScrollableMenu")]
    public static void CreateScrollableMenu()
    {
      HierarchyUtility.CheckForCanvas();
      HierarchyUtility.CheckForEventSystem();

      Canvas canvas = GameObject.FindObjectOfType<Canvas>();

      GameObject scrollableMenu = GetPrefab<ScrollableMenu>("51a79e79b906a434a861fce98f41d72a");

      GameObjectUtility.SetParentAndAlign(scrollableMenu.gameObject, canvas.gameObject);

      // Register the creation in the undo system
      Undo.RegisterCreatedObjectUndo(scrollableMenu, "Create " + scrollableMenu.name);
      Selection.activeObject = scrollableMenu;
      SceneView.lastActiveSceneView.FrameSelected();
    }

    private static GameObject GetPrefab<T>(string guid)
    {
      var path = AssetDatabase.GUIDToAssetPath(guid);
      var obj = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
      PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
      return obj;
    }
  }
}