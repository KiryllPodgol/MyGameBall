using UnityEngine;

public class PingPongPlatform : MonoBehaviour
{
    public float speed = 2.0f;      
    public float distance = 5.0f;  
    public bool reverse = false; 

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance);
        if (reverse)
        {
            offset = distance - offset;
        }
        
        transform.position = startPosition + new Vector3(0, 0, offset);
    }
}