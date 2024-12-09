using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float followSpeed = 5f; 
    private Transform target;
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void FixedUpdate()
    {
        if (target == null) return; 
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
}