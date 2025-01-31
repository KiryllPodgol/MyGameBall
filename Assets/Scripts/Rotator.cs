using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 100f;
    private float targetSpeed;
    public float smoothTime = 5f;
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
       
        float currentSpeed = Mathf.Lerp(0, rotationSpeed, Time.deltaTime * smoothTime);
        transform.Rotate(rotationAxis * currentSpeed * Time.deltaTime);
    }
}
