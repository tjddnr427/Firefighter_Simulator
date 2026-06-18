using UnityEngine;

public class FireZone : MonoBehaviour
{
    public float damageInterval = 1f;
    private float damageTimer;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                other.GetComponent<PlayerHealth>()?.TakeDamage(1);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            damageTimer = 0f;
    }
}
