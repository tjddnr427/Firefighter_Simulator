using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum ItemType { Extinguisher, Crowbar }
    public static ItemType currentItem = ItemType.Extinguisher;

    public float m_moveSpeed = 5.0f;

    private Animator m_animator;
    private InputAction m_moveAction;
    private Vector2 m_move;

    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        PlayerMove();
        UpdateActionAnimations();

        if (Keyboard.current.fKey.wasPressedThisFrame) OnInteract();
        if (Keyboard.current.eKey.wasPressedThisFrame) OnToggleInventory();
        if (Keyboard.current.digit1Key.wasPressedThisFrame) OnQuickSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) OnQuickSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) OnQuickSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) OnQuickSlot(3);
    }

    private void UpdateActionAnimations()
    {
        if (m_animator == null) return;

        bool isShooting = currentItem == ItemType.Extinguisher && Mouse.current.leftButton.isPressed;
        m_animator.SetBool("IsExtinguishing", isShooting);

        if (currentItem == ItemType.Crowbar && Mouse.current.leftButton.wasPressedThisFrame)
            m_animator.SetTrigger("Attack");
    }

    private void PlayerMove()
    {
        m_move = m_moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(m_move.x, 0, m_move.y).normalized;

        if (Keyboard.current.leftShiftKey.isPressed)
            moveDir *= 2.0f;

        Vector3 worldMove = transform.forward * moveDir.z + transform.right * moveDir.x;
        transform.Translate(worldMove * m_moveSpeed * Time.deltaTime, Space.World);

        if (m_animator != null) m_animator.SetFloat("MoveSpeed", moveDir.magnitude);
    }

    private void OnInteract() { Debug.Log("상호작용 키 입력됨"); }
    private void OnToggleInventory() { Debug.Log("아이템창 토글"); }
    private void OnQuickSlot(int index)
    {
        if (index == 0) currentItem = ItemType.Extinguisher;
        else if (index == 1) currentItem = ItemType.Crowbar;
        Debug.Log("아이템 변경: " + currentItem);
    }
}
