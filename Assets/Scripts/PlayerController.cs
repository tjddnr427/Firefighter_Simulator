using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float m_moveSpeed = 5.0f;
    public float m_jumpForce = 5.0f;

    private Animator m_animator;
    private Rigidbody m_rigidBody;
    private bool m_wasGrounded;
    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private InputAction m_moveAction;
    private Vector2 m_move;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        if (m_animator != null) m_animator.SetBool("Grounded", m_isGrounded);
        PlayerMove();
        Jumping();
        m_wasGrounded = m_isGrounded;

        if (Keyboard.current.fKey.wasPressedThisFrame) OnInteract();
        if (Keyboard.current.eKey.wasPressedThisFrame) OnToggleInventory();
        if (Keyboard.current.digit1Key.wasPressedThisFrame) OnQuickSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) OnQuickSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) OnQuickSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) OnQuickSlot(3);
    }

    private void PlayerMove()
    {
        m_move = m_moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(m_move.x, 0, m_move.y);
        moveDir = moveDir.normalized;

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            moveDir *= 2.0f;
        }

        Vector3 worldMove = transform.forward * moveDir.z + transform.right * moveDir.x;
        transform.Translate(worldMove * m_moveSpeed * Time.deltaTime, Space.World);

        if (m_animator != null) m_animator.SetFloat("MoveSpeed", moveDir.magnitude);
    }

    private void Jumping()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;
        if (jumpCooldownOver && m_isGrounded && Keyboard.current.spaceKey.isPressed)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            if (m_animator != null) m_animator.SetTrigger("Jump");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                    m_collisions.Add(collision.collider);
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
            m_collisions.Remove(collision.collider);
        if (m_collisions.Count == 0) m_isGrounded = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }
        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
                m_collisions.Add(collision.collider);
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
                m_collisions.Remove(collision.collider);
            if (m_collisions.Count == 0) m_isGrounded = false;
        }
    }

    private void OnInteract() { Debug.Log("상호작용 키 입력됨"); }
    private void OnToggleInventory() { Debug.Log("아이템창 토글"); }
    private void OnQuickSlot(int index) { Debug.Log("퀵슬롯 " + (index + 1) + " 사용"); }
}
