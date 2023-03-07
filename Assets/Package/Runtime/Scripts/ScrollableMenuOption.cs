using System.Collections.Generic;
using HGS.ScrollableMenuSystem.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace HGS.ScrollableMenuSystem
{
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(Button))]
  public class ScrollableMenuOption : MonoBehaviour
  {
    [Header("SceneRefs")]
    [SerializeField] RectTransform tabContent;
    [SerializeField] float activeSize = 180f;
    [SerializeField] float defaultSize = 100f;
    [SerializeField] float resizeDamping = 10f;

    Button _button;
    Animator _animator;
    RectTransform _rectTransform;

    float _targetWidth;

    public RectTransform TabContent => tabContent;

    public Button.ButtonClickedEvent OnClickEvent
    {
      get => _button.onClick;
      set => _button.onClick = value;
    }

    void Awake()
    {
      _rectTransform = GetComponent<RectTransform>();
      _button = GetComponent<Button>();
      _animator = GetComponent<Animator>();
      _targetWidth = defaultSize;
    }

    void Update()
    {
      var width = _rectTransform.GetWidth();
      var newWidth = Mathf.Lerp(width, _targetWidth, Time.deltaTime * resizeDamping);
      _rectTransform.SetWidth(newWidth);
    }

    public void Toggle(bool value)
    {
      _targetWidth = value ? activeSize : defaultSize;
      _animator.SetBool("IsActive", value);
    }
  }
}