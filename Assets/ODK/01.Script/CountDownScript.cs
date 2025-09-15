using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;

public class CountDownScript : MonoBehaviour
{
    [SerializeField] private ScoreScript scoreScript;
    [SerializeField] private GameObject wintext;
    [SerializeField] private RectTransform countDownPanel;   // GameObject → RectTransform
    [SerializeField] private RectTransform fadingSlide;      // GameObject → RectTransform
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private float countDownTime = 5f;
    [SerializeField] private UnityEvent onCountDownFinished;
    private float currentTime;
    private bool isCounting = false;
    //수정
    static public bool IsGameStarting = false;

    private void Awake()
    {
        countDownPanel.gameObject.SetActive(true);
        fadingSlide.gameObject.SetActive(true);
        //수정
        onCountDownFinished.AddListener(() => IsGameStarting = true);
    }

    private IEnumerator FadingSlideOpen()
    {
        isCounting = true;
        yield return new WaitForSeconds(1f);
        fadingSlide.DOAnchorPosY(3000f, 2f).SetEase(Ease.OutExpo);
    }

    public IEnumerator FadingSlideClose()
    {
        yield return new WaitForSeconds(1f);
        fadingSlide.DOAnchorPosY(0f, 2f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(2f);
        wintext.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0f, 0f, 1f);
        wintext.GetComponent<TextMeshProUGUI>().DOColor(new Color(1f, 0f, 0f, 0f), 0.8f).SetEase(Ease.InOutSine);
        int reds = scoreScript.redScore;
        int blues = scoreScript.blueScore;

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
    }

    private void Start()
    {
        StartCountDown();
    }

    public void StartCountDown()
    {
        currentTime = countDownTime;
        UpdateCountDownText();
        StartCoroutine(FadingSlideOpen());
    }

    private void Update()
    {
        if (!isCounting) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isCounting = false;

            StartCoroutine(CountDownSlideClose());
        }

        UpdateCountDownText();
    }

    private IEnumerator CountDownSlideClose()
    {
        
        yield return new WaitForSeconds(1f);
        onCountDownFinished?.Invoke();
        countDownPanel.DOAnchorPosY(3000f, 2f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(2f);
    }

    private void UpdateCountDownText()
    {
        if (currentTime <= 0f)
        {
            countDownText.text = "GO!";
            return;
        }
        countDownText.text = Mathf.CeilToInt(currentTime).ToString();
    }
}
