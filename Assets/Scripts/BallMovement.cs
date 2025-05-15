using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    private static float speed = 8f; // Начальная скорость мяча
    private Vector3 direction; // Направление движения мяча
    private bool isMoving = true; // Флаг для управления началом движения
    private Vector3 startPosition;
    public Rigidbody2D rb;
    public AudioSource crashSound;

    void Start()
    {
        // Устанавливаем начальное направление (например, вверх)
        direction = new Vector3(0, -1, 0).normalized;
        //startPosition = transform.position;
        BallManager.RegisterBall(gameObject);
        rb.velocity = direction * speed;
    }

    //void Update()
    //{
    //    if (isMoving)
    //    {
    //        // Двигаем мяч в заданном направлении
    //        transform.Translate(direction * speed * Time.deltaTime);
    //    }
    //}

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Отскок от платформы
        if (collision.gameObject.CompareTag("Paddle"))
        {
            float hitPosition = (transform.position.x - collision.transform.position.x) / collision.collider.bounds.size.x;
            direction = new Vector2(hitPosition, 1).normalized;
            rb.velocity = direction * speed;

        }

        //// Отскок от стен (горизонтальные границы)
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Инвертируем горизонтальное направление при ударе о стену
            direction.x = -direction.x;
            rb.velocity = direction * speed;
        }

        if (collision.gameObject.CompareTag("BottomBoundary"))
        {
            Destroy(gameObject);
            HpManager.TakeDamage();
            ReturnToStart();

        }
        if (collision.gameObject.CompareTag("TopBoundary"))
        {
            // Можно добавить логику потери жизни или перезапуска уровня
            direction.y = -direction.y;
            rb.velocity = direction * speed;
        }
        if (collision.gameObject.CompareTag("Block"))
        {
            direction = Vector2.Reflect(direction, collision.contacts[0].normal);
            rb.velocity = direction * speed;
            crashSound.Play();
        }
        if (collision.gameObject.CompareTag("Ball"))
        {
            direction = Vector2.Reflect(direction, collision.contacts[0].normal);
            rb.velocity = direction * speed;
        }
    }
    private void OnDisable()
    {
        BallManager.UnregisterBall(gameObject);
    }

    public void destroyClones()
    {
        GameObject[] ballClones = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject clone in ballClones)
        {
            if (clone.name.Contains("(Clone)"))
            {
                Destroy(clone);
            }
        }
    }  

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    public static void IncreaseSpeed()
    {
        speed += 2;
    }
    public static void DecreaseSpeed()
    {
        speed -= 2;
    }
    public static float GetSpeed()
    {
        return speed;
    }

    public IEnumerator ReturnToStart()
    {
        isMoving = false;
        transform.position = startPosition; // Мгновенно возвращаемся к начальной позиции
        direction = new Vector3(0, -1, 0).normalized;
        yield return new WaitForSeconds(1.5f);
        isMoving = true;
        speed = 8;
    }
}