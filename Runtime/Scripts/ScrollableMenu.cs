using System.Collections.Generic;
using System.Linq;
using HGS.ScrollableMenuSystem.Extensions;
using HGS.ScrollableMenuSystem.Strategy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HGS.ScrollableMenuSystem
{
  [ExecuteInEditMode]
  [RequireComponent(typeof(Image))]
  public class ScrollableMenu : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
  {
    [Header("Rect")]
    [SerializeField] RectTransform container;
    [SerializeField] RectTransform viewport;

    [Header("Cursor")]
    [SerializeField] RectTransform cursorBackground;
    [SerializeField] RectTransform cursorPointer;

    [Header("Tabs")]
    [SerializeField] List<ScrollableMenuOption> options = new List<ScrollableMenuOption>();
    [SerializeField] int defaultTab = 0;

    [Header("Interpolation")]
    [SerializeField] float swipeThresholdDistance = 100f;
    [SerializeField] float cursorDamping = 10f;

    public int Tab => _interpolation != null ? _interpolation.CurrentTabIndex : defaultTab;

    List<RectTransform> Tabs => options
      .Select(options => options.TabContent)
      .ToList();

    // Strategy
    TabLayout _layout;
    TabInterpolation _interpolation;

    public UnityEvent<int> onTabChange = null;

    Vector2 _startDragPos;
    RectTransform _targetCursorFollow;
    Vector2 _targetCursorPos;

    void Awake()
    {
      _layout = new TabLayout();
      _interpolation = new TabInterpolation();

      if (Application.isPlaying)
      {
        options.ForEach(option => option.OnClickEvent.AddListener(() => HandleOnOptionClicked(option)));
      }
    }

    void Start()
    {
      if (Application.isPlaying) Restart();
    }

    void Update()
    {
      if (!Application.isPlaying)
      {
        if (_layout == null) _layout = new TabLayout();
        _layout.LockElements(this, container, Tabs);
      }
      else
      {
        _interpolation.Update(container, Time.deltaTime);
        UpdateCursorPosition();
      }

      _layout.Update(container, viewport, Tabs);
    }

    private void UpdateCursorPosition()
    {
      // Follow Target
      if (_targetCursorFollow)
      {
        var position = cursorPointer.position;
        var size = cursorPointer.sizeDelta;
        position.x = Mathf.Lerp(position.x, _targetCursorFollow.position.x, cursorDamping * Time.deltaTime);
        size.x = Mathf.Lerp(size.x, _targetCursorFollow.sizeDelta.x, cursorDamping * Time.deltaTime);
        cursorPointer.position = position;
        cursorPointer.sizeDelta = size;
      }
      // Follow cursor
      else
      {
        var position = cursorPointer.anchoredPosition;
        position.x = _targetCursorPos.x;
        cursorPointer.anchoredPosition = position;
      }
    }

    private void Restart()
    {
      _interpolation.Update(container, Time.deltaTime);
      _layout.Update(container, viewport, Tabs);
      ActivateTab(defaultTab);
    }

    public void ActivateTab(int id)
    {
      var previous = _interpolation.CurrentTabIndex;
      var clampedId = Mathf.Clamp(id, 0, options.Count);
      _interpolation.Set(id, container, Tabs);
      _targetCursorFollow = (RectTransform)options[clampedId].transform;

      if (previous == clampedId) return;
      if (previous != -1) options[previous].Toggle(false);

      options[clampedId].Toggle(true);
      onTabChange?.Invoke(clampedId);
    }

    private void OnEnable()
    {
      _layout?.LockElements(this, container, Tabs);
    }

    private void OnDisable()
    {
      _layout?.Clear();
    }

    public void OnBeginDrag(PointerEventData data)
    {
      _targetCursorPos = cursorPointer.anchoredPosition;
      _targetCursorFollow = null;
      _interpolation.Stop();
      RectTransformUtility.ScreenPointToLocalPointInRectangle(container, data.position, data.pressEventCamera, out _startDragPos);
    }

    public void OnDrag(PointerEventData data)
    {
      Vector2 localCursor;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(container, data.position, data.pressEventCamera, out localCursor);

      var delta = localCursor - _startDragPos;
      var position = container.anchoredPosition;

      position.x += delta.x;
      container.anchoredPosition = position;

      _targetCursorPos.x -= delta.x / container.GetWidth() * cursorBackground.GetWidth();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      var delta = eventData.pressPosition.x - eventData.position.x;

      if (Mathf.Abs(delta) < swipeThresholdDistance)
      {
        ActivateTab(_interpolation.CurrentTabIndex);
        return;
      }

      var targetTab = delta > 0
        ? _interpolation.CurrentTabIndex + 1
        : _interpolation.CurrentTabIndex - 1;

      var betterIndex = Mathf.Clamp(targetTab, 0, options.Count - 1);

      ActivateTab(betterIndex);
    }

    private void HandleOnOptionClicked(ScrollableMenuOption option)
    {
      var id = options.IndexOf(option);
      ActivateTab(id);
    }
  }
}