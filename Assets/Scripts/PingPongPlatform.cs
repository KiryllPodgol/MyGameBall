using UnityEngine;

public class PingPongPlatform : MonoBehaviour
{
    public float speed = 2.0f;      // Скорость движения платформы
    public float distance = 5.0f;   // Положительное расстояние
    public bool reverse = false;    // Движется ли платформа в обратном направлении

    private Vector3 startPosition;

    void Start()
    {
        // Сохраняем начальную позицию платформы
        startPosition = transform.position;
    }

    void Update()
    {
        // Рассчитываем смещение на основе PingPong
        float offset = Mathf.PingPong(Time.time * speed, distance);

        // Если reverse=true, инвертируем направление
        if (reverse)
        {
            offset = distance - offset;
        }

        // Перемещаем платформу по оси Z
        transform.position = startPosition + new Vector3(0, 0, offset);
    }
}