using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 수평 이동 속도 크기를 MoveSpeed 파라미터로 전달
        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        animator.SetFloat("MoveSpeed", speed);
        animator.SetBool("IsExtinguishing", Input.GetMouseButton(0));
    }
}
