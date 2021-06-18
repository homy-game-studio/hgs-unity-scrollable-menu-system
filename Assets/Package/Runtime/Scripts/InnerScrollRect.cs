using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HGS.ScrollableMenuSystem
{
    public class InnerScrollRect : ScrollRect
    {
        [SerializeField] ScrollRect parentScroll = null;

        bool _draggingParent = false;

        bool IsPotentialParentDrag(Vector2 inputDelta)
        {
            if (parentScroll.horizontal && !parentScroll.vertical)
            {
                return Mathf.Abs(inputDelta.x) > Mathf.Abs(inputDelta.y);
            }
            if (!parentScroll.horizontal && parentScroll.vertical)
            {
                return Mathf.Abs(inputDelta.x) < Mathf.Abs(inputDelta.y);
            }
            else return true;
        }        

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            parentScroll?.OnInitializePotentialDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (IsPotentialParentDrag(eventData.delta))
            {
                parentScroll.OnBeginDrag(eventData);
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
                parentScroll.OnDrag(eventData);
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
                parentScroll.OnEndDrag(eventData);
            }
        }
    }
}
