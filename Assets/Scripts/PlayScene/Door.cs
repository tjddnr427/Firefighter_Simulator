using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 3f;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear && !isMoving && Keyboard.current.fKey.wasPressedThisFrame)
            StartCoroutine(ToggleDoor());
    }

    IEnumerator ToggleDoor()
    {
        isMoving = true;
        Quaternion from = transform.localRotation;
        Quaternion to = isOpen
            ? Quaternion.identity
            : Quaternion.Euler(0f, openAngle, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        transform.localRotation = to;
        isOpen = !isOpen;
        isMoving = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sensor"))
            playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sensor"))
            playerNear = false;
    }
}
