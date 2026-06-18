using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCamera : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    public float verticalClamp = 80f;

    public Transform playerBody;
    private float verticalRotation = 0f;

    public float bobFrequency = 2f;
    public float bobAmplitude = 0.05f;
    private float bobTimer = 0f;
    private Vector3 defaultLocalPos;
    private Vector3 lastPlayerPos;

    void Start()
    {
        if (playerBody == null)
            playerBody = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultLocalPos = transform.localPosition;
        lastPlayerPos = playerBody.position;
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        playerBody.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity * Time.deltaTime * 100f);

        verticalRotation -= mouseDelta.y * mouseSensitivity * Time.deltaTime * 100f;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        HandleHeadBob();
    }

    void HandleHeadBob()
    {
        Vector3 flatCurrent = new Vector3(playerBody.position.x, 0, playerBody.position.z);
        Vector3 flatLast = new Vector3(lastPlayerPos.x, 0, lastPlayerPos.z);
        float speed = (flatCurrent - flatLast).magnitude / Time.deltaTime;
        lastPlayerPos = playerBody.position;

        if (speed > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            transform.localPosition = defaultLocalPos + new Vector3(0, Mathf.Sin(bobTimer) * bobAmplitude, 0);
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * 5f);
        }
    }
}
