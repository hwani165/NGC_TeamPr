using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image),typeof(Outline),typeof(BoxCollider2D))]
public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField] private Page _page;
    [SerializeField] private UnityEvent _Clicked;

    public event Action ClickEffectEnd;

    private RectTransform _recTransform;

    private Image _image;
    private Outline _outLine;

    public bool IsSuccessWorking = false;

    private bool isShaking = false;
    private void Awake()
    {
        _recTransform = GetComponent<RectTransform>();

        _image = GetComponent<Image>();
        _image.color = Color.white;

        _outLine = GetComponent<Outline>();

        _Clicked.AddListener(_clickEffect);

        if(_page != null)
        {
            if(_Clicked.GetPersistentEventCount() == 0)
            {
                Debug.Log($"<color=green>subscribe : {gameObject.name}</color>");

                _Clicked.AddListener(() => Page.UpdateOnPage(_page));
                _Clicked.AddListener(() => Page.Move(_page));
            }
            else
            {
                Debug.Log($"<color=red>subscribe working f : {gameObject.name}</color>");

                _Clicked.AddListener(() => Page.UpdateOnPage(_page, IsSuccessWorking));
                _Clicked.AddListener(() => Page.Move(_page, IsSuccessWorking));
            }
        }
    }

    #region Pointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        _outLine.effectColor = new Color(0, 255, 100, 255);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = Color.white;
        _outLine.effectColor = Color.white;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        _Clicked?.Invoke();

        _image.color = Color.white;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _image.color = Color.gray;
    }
    #endregion

    private void _clickEffect()
    {
        if (!isShaking)
        {
            isShaking = true;
            _recTransform.DOShakeRotation(1f, new Vector3(0, 0, 10f), 15, 1, true).OnComplete(() => isShaking = false);
            ClickEffectEnd?.Invoke();
        }
    }
}
