using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidingScript : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject targetObject;

    public float speed = 3f;
    private Vector3 direction;
    void Start()
    {
        targetObject = GameObject.Find("platform1");
        direction = new Vector3(0, -1, 0).normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Paddle") && PlatformMovement.GetSize() == 2)
        {
            PlatformMovement.SetSize(PlatformMovement.GetSize() + 1);
            targetObject.transform.localScale = new Vector3(1.63f, 1.63f, 1.63f);
            ScoreManager.AddScore(5);
            Destroy(gameObject);
        }
        if (collision.CompareTag("Paddle") && PlatformMovement.GetSize() == 1)
        {
            PlatformMovement.SetSize(PlatformMovement.GetSize() + 1);
            targetObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            ScoreManager.AddScore(5);
            Destroy(gameObject);
        }
    }
}
