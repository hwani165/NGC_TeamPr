using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float maxTime = 180f;
    [SerializeField] private float elapsedTime;
    private bool isRunning;
    private bool isReversed = true;
    [SerializeField] private GameObject countdownTimer;
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

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        if (elapsedTime > maxTime - 30)
        {
            timerText.color = Color.red;
        }
        if (elapsedTime > maxTime - 5)
        {
            countdownTimer.SetActive(true);
            StartCoroutine(CountDown((int)maxTime - (int)elapsedTime));
        }
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public IEnumerator CountDown(int i)
    {
        countdownTimer.GetComponent<TextMeshProUGUI>().DOKill();
        countdownTimer.GetComponent<TextMeshProUGUI>().text = i.ToString();
        countdownTimer.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0f, 0f, 1f);
        countdownTimer.GetComponent<TextMeshProUGUI>().DOColor(new Color(1f,0f,0f,0f), 0.8f).SetEase(Ease.OutElastic);

        yield return new WaitForSeconds(1f);
    }

}
