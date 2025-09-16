using TMPro;
using UnityEngine;
using DG.Tweening; // DOTween 사용을 위한 네임스페이스
using System.Collections;
using UnityEngine.Events;

public class CountDownScript : MonoBehaviour
{
    [SerializeField] private ScoreScript scoreScript; // 점수 관리 스크립트 참조
    [SerializeField] private GameObject wintext; // 승리 텍스트 UI
    [SerializeField] private RectTransform countDownPanel;   // 카운트다운 UI 패널 (RectTransform)
    [SerializeField] private RectTransform fadingSlide;      // 페이드 슬라이드 UI (RectTransform)
    [SerializeField] private TextMeshProUGUI countDownText; // 카운트다운 숫자 표시용 텍스트
    [SerializeField] private float countDownTime = 5f; // 카운트다운 초기 시간
    [SerializeField] private UnityEvent onCountDownFinished; // 카운트다운 종료 시 실행될 이벤트

    private float currentTime; // 현재 카운트다운 시간
    private bool isCounting = false; // 카운트다운 진행 여부

    // 게임 시작 여부를 다른 스크립트에서 확인 가능하도록 static으로 선언
    static public bool IsGameStarting = false;

    private void Awake()
    {
        countDownPanel.gameObject.SetActive(true); // 카운트다운 패널 활성화
        fadingSlide.gameObject.SetActive(true);    // 페이드 슬라이드 활성화
        // 카운트다운 종료 시 게임 시작 상태를 true로 설정
        onCountDownFinished.AddListener(() => IsGameStarting = true);
    }

    // 페이드 슬라이드가 화면 위로 올라가며 열리는 효과
    private IEnumerator FadingSlideOpen()
    {
        isCounting = true; // 카운트다운 시작
        yield return new WaitForSeconds(1f); // 1초 대기
        fadingSlide.DOAnchorPosY(3000f, 2f).SetEase(Ease.OutExpo); // DOTween으로 Y좌표 이동
    }

    // 페이드 슬라이드가 닫히며 승리 텍스트 표시
    public IEnumerator FadingSlideClose()
    {
        //Game.Instance.EndGame();//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        yield return new WaitForSeconds(1f); // 1초 대기
        fadingSlide.DOAnchorPosY(0f, 2f).SetEase(Ease.OutExpo); // 슬라이드 내려오기
        yield return new WaitForSeconds(2f); // 애니메이션 완료 대기

        // 승리 텍스트 초기 색상 설정
        wintext.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        // 텍스트 페이드아웃 효과
        wintext.GetComponent<TextMeshProUGUI>().DOColor(new Color(1f, 1f, 1f, 1f), 1f).SetEase(Ease.InOutSine);

        int reds = scoreScript.redScore;   // Red 팀 점수
        int blues = scoreScript.blueScore; // Blue 팀 점수
  

        // 점수 비교 후 승리자 또는 무승부 표시
        if (reds > blues)
        {
            wintext.GetComponent<TextMeshProUGUI>().text = GameObject.Find("P1").GetComponent<Player>().Nickname + " Win!";
        }
        else if (blues > reds)
        {
            wintext.GetComponent<TextMeshProUGUI>().text = GameObject.Find("P2").GetComponent<Player>().Nickname + " Win!";
        }
        else
        {
            wintext.GetComponent<TextMeshProUGUI>().text = "Draw!";
        }
        

        yield return new WaitForSeconds(5f);
    }

    private void Start()
    {
        StartCountDown(); // 게임 시작 시 카운트다운 시작
    }

    public void StartCountDown()
    {
        currentTime = countDownTime; // 현재 시간 초기화
        UpdateCountDownText(); // UI 업데이트
        StartCoroutine(FadingSlideOpen()); // 슬라이드 열기 애니메이션 시작
    }

    private void Update()
    {
        if (!isCounting) return; // 카운트다운 중이 아니면 종료

        currentTime -= Time.deltaTime; // 시간 감소

        if (currentTime <= 0f) // 카운트다운 종료 시
        {
            currentTime = 0f;
            isCounting = false;
            StartCoroutine(CountDownSlideClose()); // 슬라이드 닫기 애니메이션 시작
        }

        UpdateCountDownText(); // UI 업데이트
    }

    // 카운트다운 종료 후 패널 닫고 이벤트 실행
    private IEnumerator CountDownSlideClose()
    {
        yield return new WaitForSeconds(1f); // 대기
        onCountDownFinished?.Invoke(); // 카운트다운 종료 이벤트 호출
        countDownPanel.DOAnchorPosY(3000f, 2f).SetEase(Ease.OutExpo); // 패널 위로 이동
        yield return new WaitForSeconds(2f); // 애니메이션 완료 대기
    }

    // 카운트다운 텍스트 업데이트
    private void UpdateCountDownText()
    {
        if (currentTime <= 0f)
        {
            countDownText.text = "GO!"; // 0초 도달 시 GO! 표시
            return;
        }
        countDownText.text = Mathf.CeilToInt(currentTime).ToString(); // 남은 시간 표시 (올림)
    }
}
