using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlockScript : MonoBehaviour
{
    private static float speed = 2f; // Начальная скорость мяча
    private bool opposite = false;
    private Transform obj;
    void Start()
    {
        obj = GetComponent<Transform>();
    }

    void Update()
    {

        float moveSpeed = speed * Time.deltaTime;
        if (!opposite)
        {
            obj.position = Vector3.MoveTowards(obj.position, new Vector3(obj.position.x, obj.position.y + speed, obj.position.z), moveSpeed);
        }
        else
        {
            obj.position = Vector3.MoveTowards(obj.position, new Vector3(obj.position.x, obj.position.y - speed, obj.position.z), moveSpeed);
        }
        if (obj.position.y >= 4.57)
        {
            opposite = true;
        }
        if (obj.position.y <= 1.5)
        {
            opposite = false;
        }
    }
}
