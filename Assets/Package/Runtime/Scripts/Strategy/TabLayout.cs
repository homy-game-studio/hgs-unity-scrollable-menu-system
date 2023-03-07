using System.Collections.Generic;
using HGS.ScrollableMenuSystem.Extensions;
using UnityEngine;

namespace HGS.ScrollableMenuSystem.Strategy
{
  public class TabLayout
  {
    DrivenRectTransformTracker _driverTracker;

    public TabLayout()
    {
      _driverTracker = new DrivenRectTransformTracker();
    }

    public void LockElements(MonoBehaviour behaviour, RectTransform content, List<RectTransform> tabs)
    {
      Clear();
      if (content != null)
      {
        _driverTracker.Add(behaviour, content, DrivenTransformProperties.SizeDelta);
        _driverTracker.Add(behaviour, content, DrivenTransformProperties.Anchors);
        _driverTracker.Add(behaviour, content, DrivenTransformProperties.Pivot);
      }

      tabs.ForEach(tab =>
      {
        if (tab == null) return;
        _driverTracker.Add(behaviour, tab, DrivenTransformProperties.AnchoredPosition);
        _driverTracker.Add(behaviour, tab, DrivenTransformProperties.SizeDelta);
        _driverTracker.Add(behaviour, tab, DrivenTransformProperties.Anchors);
        _driverTracker.Add(behaviour, tab, DrivenTransformProperties.Pivot);
      });
    }

    public void Update(RectTransform content, RectTransform viewport, List<RectTransform> tabs)
    {
      if (content == null) return;
      if (viewport == null) return;

      var width = viewport.GetWidth();
      var totalWidth = width * tabs.Count;
      var height = viewport.GetHeight();

      content.pivot = new Vector2(0, 1);
      content.anchorMin = Vector2.zero;
      content.anchorMax = Vector2.one;

      content.SetSize(totalWidth, height);

      var position = Vector2.zero;

      tabs.ForEach(tab =>
      {
        if (tab == null) return;
        tab.pivot = new Vector2(0, 1);
        tab.anchorMin = Vector2.zero;
        tab.anchorMax = Vector2.one;

        tab.SetSize(width, height);
        tab.anchoredPosition = position;

        position.x += width;
      });
    }

    public void Clear()
    {
      _driverTracker.Clear();
    }
  }
}
