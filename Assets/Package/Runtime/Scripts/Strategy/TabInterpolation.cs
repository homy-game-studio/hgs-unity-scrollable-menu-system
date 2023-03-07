using System.Collections.Generic;
using HGS.ScrollableMenuSystem.Extensions;
using UnityEngine;

namespace HGS.ScrollableMenuSystem.Strategy
{
  public class TabInterpolation
  {
    private const float SPEED = 15;
    private const float MIN_SQR_MAGNITUDE = 0.25f;

    int _currentTabIndex = -1;
    Vector2 _destination = Vector2.zero;
    bool _isTabReachedPos = true;

    public int CurrentTabIndex => _currentTabIndex;

    public void Update(RectTransform content, float deltaTime)
    {
      if (content == null) return;
      if (_isTabReachedPos) return;

      content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, _destination, SPEED * deltaTime);

      if (Vector2.SqrMagnitude(content.anchoredPosition - _destination) < MIN_SQR_MAGNITUDE)
      {
        content.anchoredPosition = _destination;
        _isTabReachedPos = true;
      }
    }

    public void Stop()
    {
      _isTabReachedPos = true;
    }

    public void Set(int id, RectTransform content, List<RectTransform> tabs)
    {
      if (content == null) return;

      var width = content.GetWidth() / tabs.Count;

      _destination.x = -(id * width);
      _destination.y = content.anchoredPosition.y;
      _currentTabIndex = id;
      _isTabReachedPos = false;
    }
  }
}