using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float groundHeight = -1f;
    [SerializeField] private int jumpCooldown = 0;
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float dashDuration = 10f;
    private float jumpTimer = 0;
    private float dashTimer = 0f;
    private bool isDashing = false;

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
        if (jumpTimer <= 0 && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z)) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position += Vector3.up * jumpForce;
            jumpTimer = jumpCooldown;
        }
        if (jumpTimer > 0) jumpTimer -= Time.deltaTime;
    }
}