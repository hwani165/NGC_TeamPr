using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using System.Xml;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image hpBarImage;
    [SerializeField] private Image subHpBarImage;

    [SerializeField]  private Entity entity;
    private Tween hpTween;

    private float maxHPWidth;
    private void Start()
    {
        StartSetting();
    }
    public void StartSetting()
    {

        maxHPWidth = hpBarImage.rectTransform.sizeDelta.x;

        SetHP();
    }
    public void SetHP()
    {

        float hpRatio = entity.CurrentHp / entity.MaxHP;

        hpTween?.Kill();

        hpBarImage.rectTransform.sizeDelta = new Vector2(maxHPWidth * hpRatio, hpBarImage.rectTransform.sizeDelta.y);
        hpTween = DOTween.To(
            () => subHpBarImage.rectTransform.sizeDelta.x,
            x => subHpBarImage.rectTransform.sizeDelta = new Vector2(x, subHpBarImage.rectTransform.sizeDelta.y),
            maxHPWidth * hpRatio,
            0.25f
        ).SetEase(Ease.InOutSine);


    }

}
