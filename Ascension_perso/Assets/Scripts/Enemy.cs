using System.Collections;
using UnityEngine;

public class WolfEnemyAI : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Transform player;                      // Référence au joueur
    [SerializeField] private Transform patrolPoint1;                // Premier point de patrouille
    [SerializeField] private Transform patrolPoint2;                // Deuxième point de patrouille
    [SerializeField] private Transform attackPoint;                 // Point d'attaque
    [SerializeField] private LayerMask playerLayer;                 // Layer du joueur
    
    [Header("Recherche du joueur")]
    [SerializeField] private string playerTag = "Player";           // Tag du joueur à chercher
    [SerializeField] private bool autoFindPlayer = true;            // Chercher automatiquement le joueur
    [SerializeField] private float searchInterval = 0.5f;          // Intervalle de recherche en secondes
    
    [Header("Comportement")]
    [SerializeField] private float patrolSpeed = 2f;                // Vitesse de patrouille
    [SerializeField] private float chaseSpeed = 4f;                 // Vitesse de poursuite
    [SerializeField] private float detectionRange = 5f;             // Distance de détection du joueur
    [SerializeField] private float attackRange = 1.5f;              // Distance d'attaque
    [SerializeField] private float attackCooldown = 2f;             // Temps entre les attaques
    [SerializeField] private int attackDamage = 10;                 // Dégâts infligés par une attaque
    [SerializeField] private float patrolWaitTime = 1.5f;           // Temps d'attente aux points de patrouille
    [SerializeField] private float howlProbability = 0.1f;          // Probabilité de hurler (0-1)
    [SerializeField] private float arrivalThreshold = 0.3f;         // Distance pour considérer qu'on est arrivé au point
    
    // Sons
    [Header("Sons")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip howlSound;
    [SerializeField] private AudioClip hitSound;
    
    // Stats
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    
    // Références components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    // Variables d'état
    private enum WolfState { Patrolling, Chasing, Attacking, Stunned, WaitingAtPoint }
    private WolfState currentState;
    private Transform currentTarget;                                // Point de patrouille actuel
    private bool movingRight = true;
    private float attackTimer;
    private bool canAttack = true;
    private float stunTimer = 0f;
    private float waitTimer = 0f;
    
    // Variables pour la recherche du joueur
    private Coroutine searchCoroutine;
    
    private void Start()
    {
        // Initialiser les composants
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        // Initialiser les variables
        currentState = WolfState.Patrolling;
        currentHealth = maxHealth;
        
        // Vérifier que les points de patrouille sont assignés
        if (patrolPoint1 == null || patrolPoint2 == null)
        {
            Debug.LogError("Les points de patrouille ne sont pas assignés sur " + gameObject.name);
            enabled = false;
            return;
        }
        
        // Déterminer le point de patrouille le plus proche pour commencer
        float distanceToPoint1 = Vector2.Distance(transform.position, patrolPoint1.position);
        float distanceToPoint2 = Vector2.Distance(transform.position, patrolPoint2.position);
        
        currentTarget = distanceToPoint1 < distanceToPoint2 ? patrolPoint1 : patrolPoint2;
        
        // Démarrer les hurlements aléatoires
        StartCoroutine(RandomHowl());
        
        // Ne pas chercher automatiquement au start - attendre que le joueur spawn
        // La recherche se fera seulement sur demande
    }
    
    private void Update()
    {
        // Chercher le joueur si il n'est pas assigné et que la recherche automatique est activée
        if (player == null && autoFindPlayer && searchCoroutine == null)
        {
            StartSearchingForPlayer();
        }
        
        // Gérer le compteur de temps d'attaque
        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                canAttack = true;
            }
        }
        
        // Gérer l'état stunned
        if (currentState == WolfState.Stunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                currentState = WolfState.Patrolling;
                animator.SetBool("IsStunned", false);
            }
            return;
        }
        
        // Gérer l'attente aux points de patrouille
        if (currentState == WolfState.WaitingAtPoint)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                currentState = WolfState.Patrolling;
                animator.SetBool("IsMoving", true);
            }
            return;
        }
        
        // Vérifier si le joueur est à proximité
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Si le joueur est visible et à portée de détection
            if (distanceToPlayer <= detectionRange )
            {
                if (distanceToPlayer <= attackRange && canAttack)
                {
                    // Attaquer le joueur
                    currentState = WolfState.Attacking;
                    Attack();
                }
                else
                {
                    // Poursuivre le joueur
                    currentState = WolfState.Chasing;
                }
            }
            else if (currentState == WolfState.Chasing)
            {
                // Retourner à la patrouille si le joueur n'est plus visible
                currentState = WolfState.Patrolling;
            }
        }
        
        // Mettre à jour l'animation en fonction de l'état
        UpdateAnimation();
    }
    
    private void FixedUpdate()
    {
        // Manage Movement depending on the state
        switch (currentState)
        {
            case WolfState.Patrolling:
                Patrol();
                break;
            case WolfState.Chasing:
                ChasePlayer();
                break;
            case WolfState.Attacking:
                // No movement while attacking
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
            case WolfState.Stunned:
            case WolfState.WaitingAtPoint:
                // No movement during stun or waiting
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
        }
    }
    
    private void Patrol()
    {
        if (currentTarget == null)
            return;
            
        // Calculer la direction vers le point de patrouille actuel
        Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
        
        // Vérifier si on est arrivé au point de patrouille
        if (distanceToTarget <= arrivalThreshold)
        {
            // Changer de point de patrouille
            currentTarget = (currentTarget == patrolPoint1) ? patrolPoint2 : patrolPoint1;
            
            // Commencer à attendre
            StartWaitingAtPoint();
            return;
        }
        
        // Se déplacer vers le point de patrouille
        rb.linearVelocity = new Vector2(directionToTarget.x * patrolSpeed, rb.linearVelocity.y);
        
        // Retourner le sprite en fonction de la direction
        FlipSprite(directionToTarget.x > 0);
    }
    
    private void StartWaitingAtPoint()
    {
        currentState = WolfState.WaitingAtPoint;
        waitTimer = patrolWaitTime;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("IsMoving", false);
        
        // Possibilité de hurler quand on arrive à un point
        if (Random.value < howlProbability * 2f) // Double la chance de hurler aux points
        {
            Howl();
        }
    }
    
    private void ChasePlayer()
    {
        if (player == null)
            return;
            
        // Déterminer la direction vers le joueur
        float directionToPlayer = player.position.x > transform.position.x ? 1f : -1f;
        
        // Retourner le sprite en fonction de la direction
        FlipSprite(directionToPlayer > 0);
        
        // Déplacer le loup vers le joueur
        rb.linearVelocity = new Vector2(directionToPlayer * chaseSpeed, rb.linearVelocity.y);
    }
    
    private void Attack()
    {
        if (!canAttack)
            return;
            
        // S'orienter vers le joueur avant d'attaquer
        if (player != null)
        {
            FlipSprite(player.position.x > transform.position.x);
        }
        
        // Déclencher l'animation et le son d'attaque
        animator.SetTrigger("Attack");
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
        
        // Infliger des dégâts
        if (attackPoint != null)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            foreach (Collider2D playerCollider in hitPlayers)
            {
                // Infliger des dégâts au joueur via votre système Player
                Player playerScript = playerCollider.GetComponent<Player>();
                if (playerScript != null)
                {
                    // Appeler une méthode publique pour infliger des dégâts
                    playerScript.TakeDamageFromEnemy(attackDamage);
                }
            }
        }
        
        // Démarrer le cooldown d'attaque
        canAttack = false;
        attackTimer = attackCooldown;
        
        // Revenir à la poursuite après l'attaque
        StartCoroutine(ResetStateAfterAttack());
    }
    
    private IEnumerator ResetStateAfterAttack()
    {
        // Attendre la fin de l'animation d'attaque
        yield return new WaitForSeconds(0.5f);
        
        if (currentState == WolfState.Attacking)
        {
            if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                currentState = WolfState.Chasing;
            }
            else
            {
                currentState = WolfState.Patrolling;
            }
        }
    }
    
    private void UpdateAnimation()
    {
        // Mettre à jour les paramètres de l'Animator
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("IsMoving", isMoving && currentState != WolfState.WaitingAtPoint);
        animator.SetBool("IsChasing", currentState == WolfState.Chasing);
    }
    
    private void FlipSprite(bool faceRight)
    {
        // Retourner le sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !faceRight;
        }
    }
    
    private IEnumerator RandomHowl()
    {
        while (true)
        {
            // Attendre un temps aléatoire
            yield return new WaitForSeconds(Random.Range(8f, 15f));
            
            // Hurler avec une certaine probabilité
            if (Random.value < howlProbability && 
                currentState != WolfState.Stunned && 
                currentState != WolfState.Attacking)
            {
                Howl();
            }
        }
    }
    
    private void Howl()
    {
        if (howlSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(howlSound);
        }
        if (animator != null)
        {
            animator.SetTrigger("Howl");
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Jouer l'animation et le son de dégât
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        
        // Vérifier si le loup est mort
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Être étourdi
            currentState = WolfState.Stunned;
            stunTimer = 0.5f;
            if (animator != null)
            {
                animator.SetBool("IsStunned", true);
            }
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    
    private void Die()
    {
        // Arrêter la recherche du joueur
        StopSearchingForPlayer();
        
        // Jouer l'animation de mort
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // Désactiver les collisions et le script
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        this.enabled = false;
        
        // Arrêter le mouvement
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        
        // Détruire l'objet après un délai
        Destroy(gameObject, 2f);
    }
    
    // ========== NOUVELLES MÉTHODES POUR LA GESTION DU SPAWN ==========
    
    // Méthode pour assigner manuellement le joueur (appelée par votre fonction de spawn)
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        StopSearchingForPlayer();
        Debug.Log("Wolf " + gameObject.name + " has found player: " + player.name);
    }
    
    // Méthode pour assigner le joueur par GameObject
    public void SetPlayer(GameObject playerObject)
    {
        if (playerObject != null)
        {
            SetPlayer(playerObject.transform);
        }
    }
    
    // Commencer la recherche automatique du joueur
    public void StartSearchingForPlayer()
    {
        if (searchCoroutine == null)
        {
            searchCoroutine = StartCoroutine(SearchForPlayerCoroutine());
        }
    }
    
    // Arrêter la recherche automatique
    public void StopSearchingForPlayer()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }
    }
    
    // Coroutine qui cherche périodiquement le joueur
    private IEnumerator SearchForPlayerCoroutine()
    {
        while (player == null)
        {
            // Chercher par tag
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            
            if (playerObject != null)
            {
                SetPlayer(playerObject.transform);
                yield break; // Sortir de la coroutine
            }

            // Attendre avant de chercher à nouveau
            yield return new WaitForSeconds(searchInterval);
        }
        
        searchCoroutine = null;
    }
    
    // Méthode utilitaire pour forcer une recherche immédiate
    public void FindPlayerNow()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            SetPlayer(playerObject.transform);
        }
    }
    
    // Méthode à appeler APRÈS que le joueur ait spawné dans la scène
    public void StartHuntingPlayer()
    {
        FindPlayerNow();
        if (player == null && autoFindPlayer)
        {
            StartSearchingForPlayer();
        }
    }
    
    // Effacer la référence du joueur actuel
    public void ClearPlayer()
    {
        player = null;
        if (autoFindPlayer)
        {
            StartSearchingForPlayer();
        }
    }
    
    // ========== MÉTHODES EXISTANTES ==========
    
    // Méthodes publiques pour déboguer ou contrôler le loup
    public void SetPatrolPoints(Transform point1, Transform point2)
    {
        patrolPoint1 = point1;
        patrolPoint2 = point2;
        
        if (point1 != null && point2 != null)
        {
            // Redéterminer le point de patrouille le plus proche
            float distanceToPoint1 = Vector2.Distance(transform.position, point1.position);
            float distanceToPoint2 = Vector2.Distance(transform.position, point2.position);
            currentTarget = distanceToPoint1 < distanceToPoint2 ? point1 : point2;
        }
    }
    
    void OnDestroy()
    {
        StopSearchingForPlayer();
    }
    
    // Dessiner des gizmos pour visualiser les rayons de détection et d'attaque
    private void OnDrawGizmosSelected()
    {
        // Rayon de détection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Rayon d'attaque
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        
        // Points de patrouille et trajet
        if (patrolPoint1 != null && patrolPoint2 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolPoint1.position, 0.3f);
            Gizmos.DrawWireSphere(patrolPoint2.position, 0.3f);
            Gizmos.DrawLine(patrolPoint1.position, patrolPoint2.position);
            
            // Ligne vers le point de patrouille actuel
            if (currentTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
        
        // Ligne vers le joueur si il est détecté
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}