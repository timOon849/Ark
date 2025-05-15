using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    // Список всех активных шариков на игровом поле
    private static List<GameObject> activeBalls = new List<GameObject>();
    public GameObject ballPrefab;
    public Vector3 spawnPosition;

    // Метод вызывается при создании нового шарика
    public static void RegisterBall(GameObject ball)
    {
        
        if (!activeBalls.Contains(ball))
        {
            Debug.Log("Добавлен шарик");
            activeBalls.Add(ball);
        }
        Debug.Log($"Шариков: {activeBalls.Count}");
    }

    // Метод вызывается при уничтожении шарика
    public static void UnregisterBall(GameObject ball)
    {
        Debug.Log("Уничтожаю шарик");
        if (activeBalls.Contains(ball))
        {
            Debug.Log("Шарик уничтожен");
            activeBalls.Remove(ball);
            Debug.Log($"Шариков: {activeBalls.Count}");
            // Если больше нет активных шариков, игрок теряет жизнь
            if (activeBalls.Count == 0)
            {
                Debug.Log("Наношу урон");
                HpManager.TakeDamage();
                HpManager.SpawnNewBall(Instance.ballPrefab, Instance.spawnPosition);
            }
        }
    }

    // Синглтон для доступа к экземпляру BallManager
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