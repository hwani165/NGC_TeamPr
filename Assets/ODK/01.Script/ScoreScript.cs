using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public int redScore = 0;
    public int blueScore = 0;

    public int maxScore = 3;

    public TextMeshProUGUI redScoreText;
    public TextMeshProUGUI blueScoreText;

    public TimerScript timerScript;

    void Start()
    {
        
    }

    [ContextMenu("RedScoreUp")]
    void RedScoreUp()
    {
        redScore++;
        redScoreText.text = redScore.ToString();
        if (redScore >= maxScore)
        {
           timerScript.EndTimer();
        }
    }
    [ContextMenu("BlueScoreUp")]
    void BlueScoreUp()
    {
        blueScore++;
        blueScoreText.text = blueScore.ToString();
        if (redScore >= maxScore)
        {
            timerScript.EndTimer();
        }
    }

}
