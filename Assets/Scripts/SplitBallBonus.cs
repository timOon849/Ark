using UnityEngine;

public class SplitBallsBonus : MonoBehaviour
{
    public GameObject ballPrefab;
    public float initialForce = 5f;
    public float speed = 8f;
    private Vector3 direction;

    void Start()
    {
        direction = new Vector3(0, -1, 0).normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Paddle"))
        {
            ActivateBonus();
            ScoreManager.AddScore(5);
            Destroy(gameObject);
        }
    }


    private void ActivateBonus()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if (balls.Length <= 100)
        { 
            foreach (GameObject ball in balls)
            {
                for (int i = 0; i < 2; i++)
                {
                    SpawnNewBall(ball.transform.position);
                }
            }
        }
    }

    private void SpawnNewBall(Vector2 position)
    {
        if (ballPrefab != null)
        {
            GameObject newBall = Instantiate(ballPrefab, position, Quaternion.identity);
            Rigidbody2D rb = newBall.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction.normalized * speed;
            }
        }
        else
        {
            Debug.LogError("Ball prefab не назначен!");
        }
    }
}