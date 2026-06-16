using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalClamp = 80f;

    [SerializeField] private Transform playerBody;
    private float verticalRotation = 0f;

    void Start()
    {
        if (playerBody == null)
            playerBody = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 좌우: 플레이어 회전
        playerBody.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity * Time.deltaTime * 100f);

        // 상하: 카메라만 회전 (상하 제한)
        verticalRotation -= mouseDelta.y * mouseSensitivity * Time.deltaTime * 100f;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
