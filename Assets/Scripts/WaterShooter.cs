using UnityEngine;
using UnityEngine.InputSystem;

public class WaterShooter : MonoBehaviour
{
    public GameObject waterPrefab;
    public Transform firePoint;
    public float fireRate = 0.1f;
    public float waterLifetime = 1.5f;

    private float nextFireTime;

    void Update()
    {
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            GameObject water = Instantiate(waterPrefab, firePoint.position, firePoint.rotation);
            Destroy(water, waterLifetime);
        }
    }
}
