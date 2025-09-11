using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    [SerializeField] private float maxTime = 180f;
    [SerializeField] private float elapsedTime;
    private bool isRunning;
    private bool isCountdownStarted = false;

    [SerializeField] private GameObject countdownTimer;
    [SerializeField] private CountDownScript countDownScript;

    public bool iscount;
    private void Start()
    {
        elapsedTime = maxTime;
        timerText = GetComponent<TextMeshProUGUI>();
        timerText.color = Color.white;
        
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime -= Time.deltaTime;   // ⬅ 반전 (시간 줄어듦)
            if (elapsedTime <= 5f && !iscount)
            {
                iscount = true;
                isRunning = false;
                StartCoroutine(CountDown(5)); // 끝났을 때 카운트다운
            }
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        elapsedTime = maxTime;   // ⬅ 반전 (180에서 시작)
        isRunning = true;
        isCountdownStarted = false;
        UpdateTimerDisplay();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    [ContextMenu("Timer EndRed")]
    public void TimerEndRed()
    {
        elapsedTime = 5f; // ⬅ 반전: 남은 시간 5초로 세팅
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (elapsedTime <= 30f)
        {
            timerText.color = Color.red;
        }

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public IEnumerator CountDown(int secondsLeft)
    {
        var tmp = countdownTimer.GetComponent<TextMeshProUGUI>();
        countdownTimer.SetActive(true);

        while (secondsLeft > 0)
        {
            tmp.DOKill();
            tmp.text = secondsLeft.ToString();
            tmp.color = new Color(1f, 0f, 0f, 1f);
            tmp.DOColor(new Color(1f, 0f, 0f, 0f), 0.8f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);
            secondsLeft--;
        }

        tmp.color = new Color(1f, 0f, 0f, 1f);
        tmp.text = "FINISH!";
        StopTimer();
        StartCoroutine(countDownScript.FadingSlideClose());
    }
}
