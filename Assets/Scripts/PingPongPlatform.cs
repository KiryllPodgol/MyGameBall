using UnityEngine;

public class PingPongPlatform : MonoBehaviour
{
    public float speed = 2.0f;     
    public float distance = 5.0f;  
    public bool reverse = false;

    private Vector3 startPosition;
    private Vector3 lastPosition;

    void Start()
    {
        startPosition = transform.position;
        lastPosition = startPosition;
    }

    void FixedUpdate()
    {
        
        float offset = Mathf.PingPong(Time.time * speed, distance);
        if (reverse)
        {
            offset = distance - offset;
        }
        Vector3 newPosition = startPosition + new Vector3(0, 0, offset);
        
        Vector3 platformVelocity = (newPosition - lastPosition) / Time.deltaTime;
        
        transform.position = newPosition;
        lastPosition = newPosition;
        ApplyPlatformMovement(platformVelocity);
    }

    void ApplyPlatformMovement(Vector3 platformVelocity)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation);

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                if (rb.transform.parent != transform)
                {
                    rb.transform.SetParent(transform);
                }
                
                rb.linearVelocity = platformVelocity;
            }
        }
    }
}
