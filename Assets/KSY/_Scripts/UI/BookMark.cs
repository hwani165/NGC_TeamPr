using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class BookMark : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float _defaultX;
    private float _defaultY;

    private void Awake()
    {
        _defaultX = transform.position.x;
        _defaultY = transform.position.y;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
         transform.DOMoveY(_defaultY + 50f, 0.5f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOMoveY(_defaultY, 0.5f);
    }
}
