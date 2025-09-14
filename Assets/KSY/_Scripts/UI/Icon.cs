using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image),typeof(Outline),typeof(BoxCollider2D))]
public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField] private Page _page;

    public event Action Clicked;
    public event Action ClickEffectEnd;

    private RectTransform _recTransform;

    private Image _image;
    private Outline _outLine;

    private bool isShaking = false;
    private void Awake()
    {
        _recTransform = GetComponent<RectTransform>();

        _image = GetComponent<Image>();
        _image.color = Color.white;

        _outLine = GetComponent<Outline>();

        Clicked += _clickEffect;
        Clicked += () => Page.OnTop(_page);
        Clicked += () => Page.Move(_page);
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
        Clicked?.Invoke();

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
