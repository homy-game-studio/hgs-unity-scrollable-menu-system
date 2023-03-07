using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HGS.ScrollableMenuSystem
{
  public class ChildScrollRect : ScrollRect
  {
    [SerializeField] ScrollableMenu parentScrollable = null;

    bool _draggingParent = false;

    bool IsPotentialParentDrag(Vector2 inputDelta)
    {
      return Mathf.Abs(inputDelta.x) > Mathf.Abs(inputDelta.y);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
      if (IsPotentialParentDrag(eventData.delta))
      {
        parentScrollable.OnBeginDrag(eventData);
        _draggingParent = true;
      }
      else
      {
        base.OnBeginDrag(eventData);
      }
    }

    public override void OnDrag(PointerEventData eventData)
    {
      if (_draggingParent)
      {
        parentScrollable.OnDrag(eventData);
      }
      else
      {
        base.OnDrag(eventData);
      }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
      base.OnEndDrag(eventData);
      if (_draggingParent)
      {
        _draggingParent = false;
        parentScrollable.OnEndDrag(eventData);
      }
    }
  }
}