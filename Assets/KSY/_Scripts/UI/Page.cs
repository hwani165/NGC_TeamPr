using System;
using DG.Tweening;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class Page : MonoBehaviour
{
    private static Page _defaultPage;
    private static Page TopPage;

    private static float defaultX = -20;
    private static float defaultY = -960;

    private RectTransform _rtrf;

    private bool isMoving;
    private bool isOnTop;
    private void Awake()
    {
        _rtrf = GetComponent<RectTransform>();

        if(_defaultPage == null)
        _defaultPage = GameObject.Find("Canvas/Pages/Image_R_DefaultPage").GetComponent<Page>();

        _rtrf.anchoredPosition = new Vector2(defaultX, defaultY);
    }
    static public void OnTop(Page page)
    {
        if (page.isMoving) return;

        if (TopPage != null && TopPage != page)
        {
            if (TopPage.isMoving) return;

            TopPage.isOnTop = false;
            Move(TopPage);
        }

        if (TopPage != null && TopPage == page)
        {
            if (TopPage.isMoving) return;

            page.isOnTop = page.isOnTop ? false : true;
            _defaultPage.isOnTop = _defaultPage.isOnTop ? false : true;

            TopPage = _defaultPage;
            Move(_defaultPage);
        }
        else
        {
            page.isOnTop = page.isOnTop ? false : true;
            TopPage = page;
        }

        Debug.Log($"current Page name is {TopPage.gameObject.name}");
    }
    static public void Move(Page page)
    {
        bool isOnTop = page.isOnTop;
        bool isMoving = page.isMoving;

        RectTransform rtf = page._rtrf;

        if (isMoving) return;
        if(isOnTop)
        {
            Debug.Log($"{page.gameObject.name} is Up");

            page.isMoving = true;

            rtf.DOAnchorPosY(15f, 1f).OnComplete(() =>
            {
                page.isMoving = false;
            });
        }
        else if (!isOnTop)
        {
            Debug.Log($"{page.gameObject.name} is Down");

            page.isMoving = true;

            rtf.DOAnchorPosY(defaultY, 1f).OnComplete(() =>
            {
                page.isMoving = false;
            });
        }
    }
}
