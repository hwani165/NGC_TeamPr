using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OffscreenIndicatorManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform[] players;            // �÷��̾��
    [SerializeField] private RectTransform indicatorPrefab; // Indicator ������
    [SerializeField] private float edgeBuffer = 30f;         // ȭ�� �׵θ� ����

    private List<RectTransform> indicators = new List<RectTransform>();

    void Start()
    {
        // �÷��̾� ����ŭ Indicator ����
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

        // ȭ�� �߽� ���� ����
        Vector3 dir = (player.position - mainCamera.transform.position).normalized;

        // ȭ�� ��ǥ ���
        Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position);

        // ȭ�� ���� Ŭ����
        screenPos.x = Mathf.Clamp(screenPos.x, edgeBuffer, Screen.width - edgeBuffer);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeBuffer, Screen.height - edgeBuffer);

        indicator.position = screenPos;

        // ȭ��ǥ ȸ��
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        indicator.rotation = Quaternion.Euler(0, 0, angle);
    }
}
