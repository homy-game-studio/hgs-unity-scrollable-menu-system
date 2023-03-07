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

    public RectTransform RectTransform
    {
      get
      {
        if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
        return _rectTransform;
      }
    }

    public Animator Animator
    {
      get
      {
        if (!_animator) _animator = GetComponent<Animator>();
        return _animator;
      }
    }

    public Button Button
    {
      get
      {
        if (!_button) _button = GetComponent<Button>();
        return _button;
      }
    }

    public Button.ButtonClickedEvent OnClickEvent
    {
      get => Button.onClick;
      set => Button.onClick = value;
    }

    void Awake()
    {
      _targetWidth = defaultSize;
    }

    void Update()
    {
      var width = RectTransform.GetWidth();
      var newWidth = Mathf.Lerp(width, _targetWidth, Time.deltaTime * resizeDamping);
      RectTransform.SetWidth(newWidth);
    }

    public void Toggle(bool value)
    {
      _targetWidth = value ? activeSize : defaultSize;
      Animator.SetBool("IsActive", value);
    }
  }
}