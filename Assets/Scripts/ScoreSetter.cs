using UnityEngine;
using UnityEngine.UI; // ��� ������ � Text

public class ScoreSetter : MonoBehaviour
{
    public Text scoreText; // ���������� ��������� ������ ���� � ����������

    void Start()
    {
        // ������������� ������ �� scoreText � ScoreManager
        ScoreManager.SetScoreText(scoreText);
    }
}