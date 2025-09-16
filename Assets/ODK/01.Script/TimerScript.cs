using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    // UI에 표시될 타이머 텍스트 (00:00 형식)
    private TextMeshProUGUI timerText;

    // 최대 시간 (기본 180초 = 3분)
    [SerializeField] private float maxTime = 180f;

    // 경과 시간 (남은 시간으로 사용)
    [SerializeField] private float elapsedTime;

    // 타이머 실행 여부
    private bool isRunning;

    // 카운트다운 애니메이션이 시작되었는지 여부
    private bool isCountdownStarted = false;

    // 카운트다운 UI 오브젝트 (숫자 "5", "4", ... 보여주는 용도)
    [SerializeField] private GameObject countdownTimer;

    // 카운트다운 연출용 스크립트
    [SerializeField] private CountDownScript countDownScript;

    // 카운트다운을 한 번만 실행하기 위한 플래그
    public bool iscount;

    private void Start()
    {
        // 시작할 때 시간 초기화
        elapsedTime = maxTime;

        // 같은 오브젝트에 붙은 TextMeshProUGUI 가져오기
        timerText = GetComponent<TextMeshProUGUI>();

        // 기본 색상은 흰색
        timerText.color = Color.white;

        // 초기 텍스트 표시
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (isRunning && Server.IsSuperGamer)
        {
            // 매 프레임마다 남은 시간 감소
            elapsedTime -= Time.deltaTime;

            // 남은 시간이 5초 이하일 때, 카운트다운 실행 (한 번만 실행)
            if (elapsedTime <= 5f && !iscount)
            {
                iscount = true;
                StartCoroutine(CountDown(5));
            }

            // 화면에 시간 표시 업데이트
            UpdateTimerDisplay();
        }
    }

    // 타이머 시작
    public void StartTimer()
    {
        elapsedTime = maxTime;   // 설정한 최대시간에서 시작
        isRunning = true;
        isCountdownStarted = false;
        UpdateTimerDisplay();
    }

    // 타이머 정지
    public void StopTimer()
    {
        isRunning = false;
    }

    // 디버그용: 남은 시간을 5초로 강제 세팅
    [ContextMenu("Timer EndRed")]
    public void TimerEndRed()
    {
        elapsedTime = 5f;
        UpdateTimerDisplay();
    }

    // UI에 시간 갱신
    private void UpdateTimerDisplay()
    {
        // 분:초 단위 변환
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        // 30초 이하일 때 글자색 빨강으로 변경
        if (elapsedTime <= 30f)
        {
            timerText.color = Color.red;
        }

        // "MM:SS" 포맷으로 출력
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 타이머 끝났을 때 실행되는 연출
    public void EndTimer()
    {
        var tmp = countdownTimer.GetComponent<TextMeshProUGUI>();
        countdownTimer.SetActive(true);

        // 타이머 정지
        isRunning = false;

        // 본 타이머는 "00:00" 출력
        timerText.text = "00:00";

        // "FINISH!" 출력
        tmp.color = new Color(1f, 0f, 0f, 1f);
        tmp.text = "FINISH!";

        // 안전하게 정지
        StopTimer();

        // UI 닫기 애니메이션 실행
        StartCoroutine(countDownScript.FadingSlideClose());
    }

    // 마지막 5초 카운트다운 연출 (5,4,3,2,1 표시)
    public IEnumerator CountDown(int secondsLeft)
    {
        var tmp = countdownTimer.GetComponent<TextMeshProUGUI>();
        countdownTimer.SetActive(true);

        while (secondsLeft > 0)
        {
            tmp.DOKill(); // 이전 DOTween 효과 중지
            tmp.text = secondsLeft.ToString(); // 숫자 출력
            tmp.color = new Color(1f, 0f, 0f, 1f); // 빨강(불투명)

            // 0.8초 동안 서서히 투명해지는 효과
            tmp.DOColor(new Color(1f, 0f, 0f, 0f), 0.8f).SetEase(Ease.InOutSine);

            // 1초 대기 후 다음 숫자
            yield return new WaitForSeconds(1f);
            secondsLeft--;
        }

        // 0이 되면 EndTimer 실행
        EndTimer();
    }
}
