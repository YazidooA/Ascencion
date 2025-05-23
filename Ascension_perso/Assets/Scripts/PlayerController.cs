using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // Si vous avez des animations
    
    // Variables pour la synchronisation réseau
    private Vector3 networkPosition;
    private float networkLag = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // Initialiser la position réseau
        networkPosition = transform.position;
    }
    
    private void Update()
    {
        // Si c'est mon joueur
        if (photonView.IsMine)
        {
            ProcessInputs();
            UpdateAnimation();
        }
        // Si c'est un joueur contrôlé par quelqu'un d'autre
        else
        {
            // Interpolation lisse vers la position reçue par le réseau
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * networkLag);
        }
    }

    private void ProcessInputs()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        
        Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed;
        rb.linearVelocity = movement;
        
        // Inverser le sprite selon la direction
        if (moveX != 0)
        {
            spriteRenderer.flipX = moveX < 0;
        }
    }
    
    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // Définir les paramètres d'animation
            bool isMoving = rb.linearVelocity.sqrMagnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
            
            // Vous pouvez ajouter d'autres paramètres d'animation ici
        }
    }
    
    // Interface pour la synchronisation réseau
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Envoyer des données
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rb.linearVelocity);
            stream.SendNext(spriteRenderer.flipX);
        }
        // Recevoir des données
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            rb.linearVelocity = (Vector2)stream.ReceiveNext();
            spriteRenderer.flipX = (bool)stream.ReceiveNext();
        }
    }
}