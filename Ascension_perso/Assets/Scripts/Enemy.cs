using System.Collections;
using UnityEngine;

public class WolfEnemyAI : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform patrolPoint1;
    [SerializeField] private Transform patrolPoint2;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    [Header("Recherche du joueur")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool autoFindPlayer = true;
    [SerializeField] private float searchInterval = 0.5f;

    [Header("Comportement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float patrolWaitTime = 1.5f;
    [SerializeField] private float howlProbability = 0.1f;
    [SerializeField] private float arrivalThreshold = 0.3f;

    [Header("Sons")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip howlSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private enum WolfState { Patrolling, Chasing, Attacking, Stunned, WaitingAtPoint }
    private WolfState currentState;
    private Transform currentTarget;
    private bool movingRight = true;
    private float attackTimer;
    private bool canAttack = true;
    private float stunTimer = 0f;
    private float waitTimer = 0f;

    private Coroutine searchCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        currentState = WolfState.Patrolling;
        currentHealth = maxHealth;

        if (patrolPoint1 == null || patrolPoint2 == null)
        {
            Debug.LogError("Les points de patrouille ne sont pas assignés sur " + gameObject.name);
            enabled = false;
            return;
        }

        float distanceToPoint1 = Vector2.Distance(transform.position, patrolPoint1.position);
        float distanceToPoint2 = Vector2.Distance(transform.position, patrolPoint2.position);

        currentTarget = distanceToPoint1 < distanceToPoint2 ? patrolPoint1 : patrolPoint2;

        StartCoroutine(RandomHowl());
    }

    private void Update()
    {
        if (player == null && autoFindPlayer && searchCoroutine == null)
        {
            StartSearchingForPlayer();
        }

        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                canAttack = true;
            }
        }

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

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer <= attackRange && canAttack)
                {
                    currentState = WolfState.Attacking;
                    Attack();
                }
                else
                {
                    currentState = WolfState.Chasing;
                }
            }
            else if (currentState == WolfState.Chasing)
            {
                currentState = WolfState.Patrolling;
            }
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case WolfState.Patrolling:
                Patrol();
                break;
            case WolfState.Chasing:
                ChasePlayer();
                break;
            case WolfState.Attacking:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
            case WolfState.Stunned:
            case WolfState.WaitingAtPoint:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
        }
    }

    private void Patrol()
    {
        if (currentTarget == null)
            return;

        Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToTarget <= arrivalThreshold)
        {
            currentTarget = (currentTarget == patrolPoint1) ? patrolPoint2 : patrolPoint1;
            StartWaitingAtPoint();
            return;
        }

        rb.linearVelocity = new Vector2(directionToTarget.x * patrolSpeed, rb.linearVelocity.y);
        FlipSprite(directionToTarget.x > 0);
    }

    private void StartWaitingAtPoint()
    {
        currentState = WolfState.WaitingAtPoint;
        waitTimer = patrolWaitTime;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("IsMoving", false);

        if (Random.value < howlProbability * 2f)
        {
            Howl();
        }
    }

    private void ChasePlayer()
    {
        if (player == null)
            return;

        float directionToPlayer = player.position.x > transform.position.x ? 1f : -1f;
        FlipSprite(directionToPlayer > 0);
        rb.linearVelocity = new Vector2(directionToPlayer * chaseSpeed, rb.linearVelocity.y);
    }

    private void Attack()
    {
        if (!canAttack)
            return;

        if (player != null)
        {
            FlipSprite(player.position.x > transform.position.x);
        }

        animator.SetTrigger("Attack");
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        if (attackPoint != null)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            foreach (Collider2D playerCollider in hitPlayers)
            {
                Player playerScript = playerCollider.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamageFromEnemy(attackDamage);
                }
            }
        }

        canAttack = false;
        attackTimer = attackCooldown;
        StartCoroutine(ResetStateAfterAttack());
    }

    private IEnumerator ResetStateAfterAttack()
    {
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
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("IsMoving", isMoving && currentState != WolfState.WaitingAtPoint);
        animator.SetBool("IsChasing", currentState == WolfState.Chasing);
    }

    private void FlipSprite(bool faceRight)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !faceRight;
        }
    }

    private IEnumerator RandomHowl()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(8f, 15f));

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

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
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
        StopSearchingForPlayer();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        this.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        Destroy(gameObject, 2f);
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        StopSearchingForPlayer();
        Debug.Log("Wolf " + gameObject.name + " has found player: " + player.name);
    }

    public void SetPlayer(GameObject playerObject)
    {
        if (playerObject != null)
        {
            SetPlayer(playerObject.transform);
        }
    }

    public void StartSearchingForPlayer()
    {
        if (searchCoroutine == null)
        {
            searchCoroutine = StartCoroutine(SearchForPlayerCoroutine());
        }
    }

    public void StopSearchingForPlayer()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }
    }

    private IEnumerator SearchForPlayerCoroutine()
    {
        while (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

            if (playerObject != null)
            {
                SetPlayer(playerObject.transform);
                yield break;
            }

            yield return new WaitForSeconds(searchInterval);
        }

        searchCoroutine = null;
    }

    public void FindPlayerNow()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            SetPlayer(playerObject.transform);
        }
    }

    public void StartHuntingPlayer()
    {
        FindPlayerNow();
        if (player == null && autoFindPlayer)
        {
            StartSearchingForPlayer();
        }
    }

    public void ClearPlayer()
    {
        player = null;
        if (autoFindPlayer)
        {
            StartSearchingForPlayer();
        }
    }

    public void SetPatrolPoints(Transform point1, Transform point2)
    {
        patrolPoint1 = point1;
        patrolPoint2 = point2;

        if (point1 != null && point2 != null)
        {
            float distanceToPoint1 = Vector2.Distance(transform.position, point1.position);
            float distanceToPoint2 = Vector2.Distance(transform.position, point2.position);
            currentTarget = distanceToPoint1 < distanceToPoint2 ? point1 : point2;
        }
    }

    void OnDestroy()
    {
        StopSearchingForPlayer();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (patrolPoint1 != null && patrolPoint2 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolPoint1.position, 0.3f);
            Gizmos.DrawWireSphere(patrolPoint2.position, 0.3f);
            Gizmos.DrawLine(patrolPoint1.position, patrolPoint2.position);

            if (currentTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
