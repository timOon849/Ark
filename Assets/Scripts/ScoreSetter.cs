using UnityEngine;
using UnityEngine.UI; // Для работы с Text

public class ScoreSetter : MonoBehaviour
{
    public Text scoreText; // Перетащите текстовый объект сюда в инспекторе

    void Start()
    {
        // Устанавливаем ссылку на scoreText в ScoreManager
        ScoreManager.SetScoreText(scoreText);
    }
}