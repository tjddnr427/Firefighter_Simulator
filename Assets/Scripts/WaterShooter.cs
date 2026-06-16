using UnityEngine;
using UnityEngine.InputSystem;

public class WaterShooter : MonoBehaviour
{
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float waterLifetime = 1.5f;

    private float nextFireTime;
    private bool isFiring;

    // Player Input 컴포넌트(Send Messages)가 Fire 액션 발생 시 자동 호출
    public void OnFire(InputValue value)
    {
        isFiring = value.isPressed;
    }

    void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            GameObject water = Instantiate(waterPrefab, firePoint.position, firePoint.rotation);
            Destroy(water, waterLifetime);
        }
    }
}
