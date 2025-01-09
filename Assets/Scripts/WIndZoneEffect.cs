using UnityEngine;

public class WindZoneEffect : MonoBehaviour
{
    [SerializeField] private WindZone windZone; // Ссылка на Wind Zone
    [SerializeField] private float windForceMultiplier = 1f; // Множитель силы ветра

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && windZone != null)
        {
            Vector3 windDirection;

            // Проверяем режим Wind Zone
            if (windZone.mode == WindZoneMode.Spherical)
            {
                // Для сферического ветра направление от центра Wind Zone к объекту
                windDirection = (other.transform.position - windZone.transform.position).normalized;
            }
            else
            {
                // Для направленного ветра используем forward вектора Wind Zone
                windDirection = windZone.transform.forward;
            }

            // Сила ветра
            float windStrength = windZone.windMain;

            // Применяем силу ветра к объекту
            Vector3 windForce = windDirection * windStrength * windForceMultiplier;
            rb.AddForce(windForce, ForceMode.Acceleration);
        }
    }
}
