using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlatformerController : MonoBehaviourPunCallbacks
{
    [Header("Mouvement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    [Header("Détection de plateformes")]
    public Transform ceilingCheck;
    public float ceilingCheckRadius = 0.2f;
    
    [Header("Composants")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    // Variables privées
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;
    private bool facingRight = true;
    
    // Constantes d'animation
    private static readonly int AnimIsRunning = Animator.StringToHash("isRunning");
    private static readonly int AnimIsJumping = Animator.StringToHash("isJumping");
    private static readonly int AnimIsFalling = Animator.StringToHash("isFalling");
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Si pas de référence au spriteRenderer, essayez de le trouver
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        // Si pas de référence à l'animator, essayez de le trouver
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Ne gérer l'input que si c'est notre joueur
        if (photonView.IsMine)
        {
            // Lecture des inputs
            horizontalInput = Input.GetAxisRaw("Horizontal");
            
            // Vérifier si le bouton de saut est pressé
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                jumpPressed = true;
            }
            
            // Mettre à jour les animations
            UpdateAnimations();
        }
    }
    
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // Vérifier si le joueur touche le sol
            CheckGrounded();
            
            // Mouvement horizontal
            MoveHorizontal();
            
            // Gestion du saut
            HandleJump();
            
            // Meilleur feeling de saut (à la Super Mario)
            ApplyJumpMultipliers();
        }
    }
    
    void CheckGrounded()
    {
        // Vérifier si le joueur touche le sol avec un overlap circle
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    
    void MoveHorizontal()
    {
        // Déplacer le personnage horizontalement
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        
        // Retourner le sprite en fonction de la direction
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }
    }
    
    void HandleJump()
    {
        // Appliquer la force de saut
        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            
            // RPC pour jouer l'effet de saut sur tous les clients
            photonView.RPC("PlayJumpEffect", RpcTarget.All);
        }
    }
    
    void ApplyJumpMultipliers()
    {
        // Appliquer un multiplicateur de chute pour un meilleur feeling
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Saut plus court si le joueur relâche le bouton de saut
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    
    void Flip()
    {
        // Changer la direction du personnage
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;
    }
    
    void UpdateAnimations()
    {
        // Mettre à jour les paramètres d'animation
        if (animator != null)
        {
            animator.SetBool(AnimIsRunning, Mathf.Abs(horizontalInput) > 0.1f && isGrounded);
            animator.SetBool(AnimIsJumping, !isGrounded && rb.linearVelocity.y > 0);
            animator.SetBool(AnimIsFalling, !isGrounded && rb.linearVelocity.y < 0);
        }
    }
    
    [PunRPC]
    void PlayJumpEffect()
    {
        // Jouer un effet de saut (particules, son, etc.)
        // Exemple:
        // Instantiate(jumpParticles, transform.position, Quaternion.identity);
        // audioSource.PlayOneShot(jumpSound);
    }
    
    void OnDrawGizmos()
    {
        // Dessiner les rayons de vérification (pour le debug)
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
        }
    }
}