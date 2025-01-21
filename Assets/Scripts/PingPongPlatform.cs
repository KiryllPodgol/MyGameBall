using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float StartMovingTime;
    private float MovingTime;                      
    [SerializeField] private float MovingSpeed;   
    [SerializeField] private Vector3 moveDirection;

    private void Start()
    {
        MovingTime = StartMovingTime;           
    }
    private void FixedUpdate()
    {
        if (MovingTime <= 0)
        {
            moveDirection = -moveDirection;      
            MovingTime = StartMovingTime;
        }
        else
        {
            MovingTime -= Time.fixedDeltaTime;     
        }
        
        transform.Translate(moveDirection * MovingSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Устанавливаем родителем платформу при столкновении
        collision.transform.parent = transform;
    }

    private void OnCollisionExit(Collision collision)
    {
        
        collision.transform.parent = null;
        transform.parent = null;
    }
}