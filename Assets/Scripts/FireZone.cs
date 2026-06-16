using UnityEngine;

public class FireZone : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerHealth>()?.TakeDamage(damagePerSecond);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerHealth>()?.TakeDamage(damagePerSecond * Time.deltaTime);
    }
}
