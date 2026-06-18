using UnityEngine;
using UnityEngine.InputSystem;

public class Wreck : MonoBehaviour
{
    public int hp = 3;

    private bool sensorNear = false;
    private ParticleSystem hitParticle;

    void Start()
    {
        hitParticle = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (sensorNear
            && PlayerController.currentItem == PlayerController.ItemType.Crowbar
            && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (hitParticle != null) hitParticle.Play();

            hp--;
            if (hp <= 0) Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sensor")) sensorNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sensor")) sensorNear = false;
    }
}
