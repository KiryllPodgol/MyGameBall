using UnityEngine;

public class SlidingBeam : MonoBehaviour
{
    public float speed = 2f; 
    public float distance = 5f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; 
    }
    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance); 
        transform.position = startPosition + new Vector3(offset, 0, 0); 
    }
}