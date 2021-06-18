using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace HGS.ScrollableMenuSystem
{
    public class OutterScrollRect : ScrollRect
    {
        public Action<PointerEventData> onBeginDrag = null;
        public Action<PointerEventData> onDrag = null;
        public Action<PointerEventData> onEndDrag = null;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            onBeginDrag?.Invoke(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            onEndDrag?.Invoke(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            onDrag?.Invoke(eventData);
        }
    }
}
