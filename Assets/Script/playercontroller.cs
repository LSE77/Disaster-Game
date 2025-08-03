using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 10f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 1.2f;
    public float extraGravity = 30f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool jumpRequest;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // ���� �߷� ��ȭ
        Physics.gravity = new Vector3(0, -20f, 0);
    }

    void Update()
    {
        // ���� �Է� ����
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        ApplyExtraGravity();

        // ���� ó�� (���� �����ӿ��� AddForce)
        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

        Vector3 targetVelocity = move * moveSpeed;
        Vector3 velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        rb.velocity = velocity;
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // ���� �� Y�ӵ� �ʱ�ȭ
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void ApplyExtraGravity()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
    }

    void CheckGround()
    {
        // �� ��ġ���� ���� �Ʒ��� Raycast
        Vector3 rayOrigin = transform.position + Vector3.down * 0.5f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, LayerMask.GetMask("Default", "Ground"));

        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = false;
    }
}
