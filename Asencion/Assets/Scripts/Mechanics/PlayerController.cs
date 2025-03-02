using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundHeight = -1f;
    [SerializeField] private int jumpCooldown = 0;
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float dashDuration = 10f;
    private float jumpTimer = 0;
    private float dashTimer = 0f;
    private bool isDashing = false;
     private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
    }

    void Update()
    {
        HandleDash();
        Deplacement();
        Sauter();
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isDashing) (isDashing, dashTimer) = (true, dashDuration);
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            isDashing = dashTimer > 0;
        }
    }

    void Deplacement()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            move -= transform.right;
            if (Input.GetKey(KeyCode.F))
            {
                transform.position += transform.right * (-5 * (speed * Time.deltaTime));
                return;
            }
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            move += transform.right;
            if (Input.GetKey(KeyCode.F))
            {
                transform.position += transform.right * (5 * (speed * Time.deltaTime));
                return;
            }
        }
        transform.position += move * ((isDashing ? speed + dashSpeed : speed) * Time.deltaTime);
    }

    void Sauter()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}