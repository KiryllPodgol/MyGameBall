using UnityEngine;

[System.Serializable]
public class CameraSettings
{
    public float mouseSensitivity = 0f;
    public Vector2 pitchLimits = new Vector2(-45f, 45f);
    public float followDistance = 10f;
    public float followHeight = 40f; 
    public float followSpeed = 50f; 
    public float cameraRadius = 0.5f;
    public LayerMask obstacleLayers;
}