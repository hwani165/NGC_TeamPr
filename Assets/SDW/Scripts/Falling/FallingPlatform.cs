using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float durationTime = 2f;
    [SerializeField] private Color blinkColor = Color.red; // ������ ����

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float initialDuration;
    private bool start = false;
    private bool isBlinking = false;

    private void Start()
    {
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
            StopAllCoroutines(); // ������ ����
            StartCoroutine(Fall());
        }
    }

    IEnumerator Blink()
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

    IEnumerator Fall()
    {
        rb.isKinematic = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
