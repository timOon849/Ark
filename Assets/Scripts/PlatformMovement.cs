using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private Transform obj;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _moveStep;
    private static int _sizeLvl = 2;
    private void Awake()
    {
        obj = GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        float moveStepSpeed = _moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            if (obj.position.x > -8)
            {
                obj.position = Vector3.MoveTowards(obj.position, new Vector2(obj.position.x - _moveSpeed, obj.position.y), moveStepSpeed);
            }
        if (Input.GetKey(KeyCode.D))
            if (obj.position.x < 8)
            {
                obj.position = Vector3.MoveTowards(obj.position, new Vector2(obj.position.x + _moveSpeed, obj.position.y), moveStepSpeed);
            }
    }
    public static int GetSize()
    {
        return _sizeLvl;
    }
    public static void SetSize(int sizeC)
    {
        _sizeLvl = sizeC;
    }
}
