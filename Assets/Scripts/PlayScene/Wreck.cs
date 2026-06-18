using UnityEngine;
using UnityEngine.InputSystem;

public class Wreck : MonoBehaviour
{
    private bool sensorNear = false;

    void Update()
    {
        if (sensorNear
            && PlayerController.currentItem == PlayerController.ItemType.Crowbar
            && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Destroy(gameObject);
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
