using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static int Scores = 0;
    public static Text scoreText;

    public static void AddScore(int sc)
    {
        Scores += sc;
        UpdateScoreCanvas();
    }

    public static int GetScore()
    {
        return Scores;
    }

    public static void UpdateScoreCanvas()
    {
        scoreText.text = $"Score: {Scores}";
    }
    
    public static void SetScoreText(Text text)
    {
        scoreText = text;
    }
}
