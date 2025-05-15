using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HpManager : MonoBehaviour
{
    private static int hp = 3;
    private static float restartDelay = 3f;
    public static string sceneToLoad = "";
    public static HpManager Instance;

    public static void TakeDamage()
    {
        LoseHp();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private static void LoseHp()
    {
        RemoveVisual(hp);
        hp--;
        if (hp <= 0)
        {
            hp = 0;
            RestartGame();
        }
    }

    private static IEnumerator RestartGameCoroutine()
    {
        yield return new WaitForSeconds(restartDelay);

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private static void RemoveVisual(int heartNum)
    {
        GameObject removable = GameObject.Find("heart3");
        if (heartNum == 3)
        {
             removable = GameObject.Find("heart3");
        }
        if (heartNum == 2)
        {
             removable = GameObject.Find("heart2");
        }
        if (heartNum == 1)
        {
             removable = GameObject.Find("heart1");
        }
        Destroy(removable);
    }

    private static void RestartGame()
    {
        GameObject anyObject = new GameObject("TempCoroutineRunner");
        anyObject.AddComponent<CoroutineRunner>().Run(RestartGameCoroutine());
    }

    public static void SpawnNewBall(GameObject ballPrefab, Vector3 spawnPosition)
    {
        Instance.StartCoroutine(SpawnNewBallWithDelay(ballPrefab, spawnPosition));
    }

    public static IEnumerator SpawnNewBallWithDelay(GameObject ballPrefab, Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(2f);
        // Создаем новый шарик в точке спавна
        GameObject newBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

        // Регистрируем новый шарик в BallManager
        BallManager.RegisterBall(newBall);
    }
}

public class CoroutineRunner : MonoBehaviour
{
    public void Run(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}