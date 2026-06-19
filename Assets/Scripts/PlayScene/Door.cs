using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 3f;
    public GameObject interactionUI;

    [Header("Backdraft")]
    public GameObject firePrefab;
    public Transform[] fireBackdraftPoints;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool playerNear = false;
    private bool backdraftTriggered = false;

    void Update()
    {
        if (playerNear && !isMoving && Keyboard.current.fKey.wasPressedThisFrame)
            StartCoroutine(ToggleDoor());
    }

    IEnumerator ToggleDoor()
    {
        isMoving = true;
        bool shouldBackdraft = !isOpen && !backdraftTriggered;
        if (shouldBackdraft) backdraftTriggered = true;

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

        if (shouldBackdraft) SpawnBackdraftFires();
    }

    void SpawnBackdraftFires()
    {
        if (firePrefab == null || fireBackdraftPoints == null) return;

        foreach (Transform point in fireBackdraftPoints)
        {
            if (point != null)
                Instantiate(firePrefab, point.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sensor"))
        {
            playerNear = true;
            if (interactionUI != null) interactionUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sensor"))
        {
            playerNear = false;
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }
}
