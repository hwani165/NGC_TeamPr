using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [Header("레이어")]
    [SerializeField] private LayerMask PlayerLayerMask;

    [Header("지속시간 & 색 설정")]
    [SerializeField] private float durationTime = 2f;
    [SerializeField] private Color blinkColor = Color.red; // 깜빡일 색상

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int PlayerLayer;
    private float initialDuration;
    private bool start = false;
    private bool isBlinking = false;

    //network data
    private bool isstep = false;
    private bool isfall = false;

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
        #region network data
        isstep = true;
        #endregion

        // 처음부터 아주 느리게 깜빡이기 시작
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
            float interval = 1f; // 기본 1초 간격

            // 남은 시간 비율에 따라 속도 변경
            if (durationTime <= initialDuration * 0.25f)
                interval = 0.25f;
            else if (durationTime <= initialDuration * 0.5f)
                interval = 0.5f;

            // 깜빡임
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
        #region network data
        isfall = true;
        #endregion
        rb.gravityScale = 1f; // 중력 적용
        yield return new WaitForSeconds(0.5f); // 0.5초 후에 파괴
        Destroy(gameObject);
    }
}
