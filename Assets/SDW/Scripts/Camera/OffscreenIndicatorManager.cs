using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OffscreenIndicatorManager : MonoBehaviour
{
    public enum ColorType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta,
        White,
        Black,
        Gray
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform[] players;
    [SerializeField] private RectTransform indicatorPrefab;
    [SerializeField] private float edgeBuffer = 30f;
    [SerializeField] private ColorType indicatorColor; // 단일 색상 선택

    private List<RectTransform> indicators = new List<RectTransform>();
    private List<Coroutine> blinkCoroutines = new List<Coroutine>();

    void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            var indicator = Instantiate(indicatorPrefab, transform);
            indicator.gameObject.SetActive(false);
            indicators.Add(indicator);
            blinkCoroutines.Add(null);

            // 단일 색상 적용
            var img = indicator.GetComponent<Image>();
            if (img != null)
            {
                img.color = GetColorFromType(indicatorColor);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            UpdateIndicator(players[i], indicators[i], i);
        }
    }

    void UpdateIndicator(Transform player, RectTransform indicator, int index)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(player.position);
        bool isOffscreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

        if (!isOffscreen)
        {
            if (indicator.gameObject.activeSelf)
            {
                indicator.gameObject.SetActive(false);
                if (blinkCoroutines[index] != null)
                {
                    StopCoroutine(blinkCoroutines[index]);
                    blinkCoroutines[index] = null;
                }
                SetIndicatorAlpha(indicator, 1f);
            }
            return;
        }

        if (!indicator.gameObject.activeSelf)
        {
            indicator.gameObject.SetActive(true);
            if (blinkCoroutines[index] == null)
                blinkCoroutines[index] = StartCoroutine(BlinkIndicator(indicator));
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position);
        screenPos.x = Mathf.Clamp(screenPos.x, edgeBuffer, Screen.width - edgeBuffer);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeBuffer, Screen.height - edgeBuffer);
        indicator.position = screenPos;

        indicator.rotation = Quaternion.identity;
    }

    IEnumerator BlinkIndicator(RectTransform indicator)
    {
        Image img = indicator.GetComponent<Image>();
        if (img == null) yield break;

        while (true)
        {
            SetIndicatorAlpha(indicator, 1f);
            yield return new WaitForSeconds(0.3f);
            SetIndicatorAlpha(indicator, 0.2f);
            yield return new WaitForSeconds(0.3f);
        }
    }

    void SetIndicatorAlpha(RectTransform indicator, float alpha)
    {
        Image img = indicator.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }

    Color GetColorFromType(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red: return Color.red;
            case ColorType.Green: return Color.green;
            case ColorType.Blue: return Color.blue;
            case ColorType.Yellow: return Color.yellow;
            case ColorType.Cyan: return Color.cyan;
            case ColorType.Magenta: return Color.magenta;
            case ColorType.White: return Color.white;
            case ColorType.Black: return Color.black;
            case ColorType.Gray: return Color.gray;
            default: return Color.white;
        }
    }
}
