using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    // ������ ���� �������� ������� �� ������� ����
    private static List<GameObject> activeBalls = new List<GameObject>();
    public GameObject ballPrefab;
    public Vector3 spawnPosition;

    // ����� ���������� ��� �������� ������ ������
    public static void RegisterBall(GameObject ball)
    {
        
        if (!activeBalls.Contains(ball))
        {
            Debug.Log("�������� �����");
            activeBalls.Add(ball);
        }
        Debug.Log($"�������: {activeBalls.Count}");
    }

    // ����� ���������� ��� ����������� ������
    public static void UnregisterBall(GameObject ball)
    {
        Debug.Log("��������� �����");
        if (activeBalls.Contains(ball))
        {
            Debug.Log("����� ���������");
            activeBalls.Remove(ball);
            Debug.Log($"�������: {activeBalls.Count}");
            // ���� ������ ��� �������� �������, ����� ������ �����
            if (activeBalls.Count == 0)
            {
                Debug.Log("������ ����");
                HpManager.TakeDamage();
                HpManager.SpawnNewBall(Instance.ballPrefab, Instance.spawnPosition);
            }
        }
    }

    // �������� ��� ������� � ���������� BallManager
    private static BallManager instance;
    public static BallManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}