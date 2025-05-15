using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseSpeed : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 direction;

    void Start()
    {
        direction = new Vector3(0, -1, 0).normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Paddle"))
        {
            if (BallMovement.GetSpeed() > 4)
            {
                BallMovement.DecreaseSpeed();
                ScoreManager.AddScore(5);
                Destroy(gameObject);
            }
        }
    }
}
