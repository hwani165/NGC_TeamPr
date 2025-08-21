using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OffscreenIndicatorManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform[] players;            // 플레이어들
    [SerializeField] private RectTransform indicatorPrefab; // Indicator 프리팹
    [SerializeField] private float edgeBuffer = 30f;         // 화면 테두리 여백

    private List<RectTransform> indicators = new List<RectTransform>();

    void Start()
    {
        // 플레이어 수만큼 Indicator 생성
        foreach (var player in players)
        {
            var indicator = Instantiate(indicatorPrefab, transform);
            indicator.gameObject.SetActive(false);
            indicators.Add(indicator);
        }
    }

    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            UpdateIndicator(players[i], indicators[i]);
        }
    }

    void UpdateIndicator(Transform player, RectTransform indicator)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(player.position);

        bool isOffscreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

        if (!isOffscreen)
        {
            indicator.gameObject.SetActive(false);
            return;
        }

        indicator.gameObject.SetActive(true);

        // 화면 중심 기준 방향
        Vector3 dir = (player.position - mainCamera.transform.position).normalized;

        // 화면 좌표 계산
        Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position);

        // 화면 끝에 클램프
        screenPos.x = Mathf.Clamp(screenPos.x, edgeBuffer, Screen.width - edgeBuffer);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeBuffer, Screen.height - edgeBuffer);

        indicator.position = screenPos;

        // 화살표 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        indicator.rotation = Quaternion.Euler(0, 0, angle);
    }
}
