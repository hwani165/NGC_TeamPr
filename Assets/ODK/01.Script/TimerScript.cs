using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class TimerScript : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float maxTime = 180f;
    [SerializeField] private float elapsedTime;
    private bool isRunning;
    private bool isReversed = true;
    [SerializeField] private GameObject countdownTimer;
    private bool isCountdownStarted = false;

    [SerializeField] private CountDownScript countDownScript;
    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        
        UpdateTimerDisplay();
    }

    private void Update()
    {

        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
        UpdateTimerDisplay();
    }

    public void StopTimer()
    {
        isRunning = false;
    }
    [ContextMenu("Timer EndRed")]
    public void TimerEndRed()
    {
        elapsedTime = maxTime - 8;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (elapsedTime > maxTime - 30)
        {
            timerText.color = Color.red;
        }

        if (!isCountdownStarted && elapsedTime > maxTime - 5)
        {
            isCountdownStarted = true;
            countdownTimer.SetActive(true);
            StartCoroutine(CountDown(5));
        }

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public IEnumerator CountDown(int secondsLeft)
    {
        var tmp = countdownTimer.GetComponent<TextMeshProUGUI>();
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
