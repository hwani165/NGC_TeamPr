using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [Header("���̾�")]
    [SerializeField] private LayerMask PlayerLayerMask;

    [Header("���ӽð� & �� ����")]
    [SerializeField] private float durationTime = 2f;
    [SerializeField] private Color blinkColor = Color.red; // ������ ����

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int PlayerLayer;
    private float initialDuration;
    private bool start = false;
    private bool isBlinking = false;

    private void Start()
    {
        PlayerLayer = Mathf.RoundToInt(Mathf.Log(PlayerLayerMask.value, 2));
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        initialDuration = durationTime;
    }

    private void Update()
    {
        if (start)
            duration();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer != PlayerLayer)
            return;
        start = true;

        // ó������ ���� ������ �����̱� ����
        if (!isBlinking)
            StartCoroutine(Blink());
    }

    private void duration()
    {
        if (durationTime > 0)
        {
            durationTime -= Time.deltaTime;
        }
        else
        {
            durationTime = 0f;
            StopAllCoroutines();
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Blink()
    {
        isBlinking = true;
        Color originalColor = sr.color;

        while (durationTime > 0)
        {
            float interval = 1f; // �⺻ 1�� ����

            // ���� �ð� ������ ���� �ӵ� ����
            if (durationTime <= initialDuration * 0.25f)
                interval = 0.25f;
            else if (durationTime <= initialDuration * 0.5f)
                interval = 0.5f;

            // ������
            sr.color = blinkColor;
            yield return new WaitForSeconds(interval / 2f);
            sr.color = originalColor;
            yield return new WaitForSeconds(interval / 2f);
        }

        sr.color = originalColor;
        isBlinking = false;
    }

    private IEnumerator Fall()
    {
        rb.gravityScale = 1f; // �߷� ����
        yield return new WaitForSeconds(0.5f); // 0.5�� �Ŀ� �ı�
        Destroy(gameObject);
    }
}
