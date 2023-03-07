using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Presets;

namespace HGS.ScrollableMenuSystem.EditorExtensions
{
  public class HierarchyUtility
  {
    public static void CheckForCanvas()
    {
      Canvas canvas = Object.FindObjectOfType<Canvas>();

      if (!canvas)
      {
        canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        Preset canvasPreset = (Preset)AssetDatabase.LoadAssetAtPath("Assets/Package/Runtime/Presets/Canvas.preset", typeof(Preset));
        Preset canvasScaler = (Preset)AssetDatabase.LoadAssetAtPath("Assets/Package/Runtime/Presets/CanvasScaler.preset", typeof(Preset));
        Preset canvasRenderer = (Preset)AssetDatabase.LoadAssetAtPath("Assets/Package/Runtime/Presets/GraphicRaycaster.preset", typeof(Preset));

        canvasPreset.ApplyTo(canvas);
        canvasScaler.ApplyTo(canvas.gameObject.AddComponent<CanvasScaler>());
        canvasRenderer.ApplyTo(canvas.gameObject.AddComponent<CanvasRenderer>());
      }
    }

    public static void CheckForEventSystem()
    {
      // Check if there is an EventSystem in the scene (if not, add one)
      EventSystem es = Object.FindObjectOfType<EventSystem>();
      if (ReferenceEquals(es, null))
      {
        GameObject esGameObject = new GameObject("EventSystem");
        esGameObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                esGameObject.AddComponent<InputSystemUIInputModule>();
#else
        esGameObject.AddComponent<StandaloneInputModule>();
#endif
      }
    }
  }
}