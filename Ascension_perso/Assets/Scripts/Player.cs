using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;

    [SerializeField] private GameObject textShop;
    [SerializeField] private Transform collisionCheck;
    [SerializeField] private LayerMask shopLayer;

    private int Money => savedDatas.Money;
    [SerializeField] private int coinValue = 5;

    private int Hp => savedDatas.Hp;
    [SerializeField] private int damages = 20;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject gameOver;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private ParticleSystem bloodParticles;

    private double CurrentLevel => savedDatas.CurrentLevel;
    [SerializeField] private LayerMask flagLayer;
    [SerializeField] private GameObject textFlagNext;

    [SerializeField] private GameObject text05;
    [SerializeField] private GameObject text15;
    [SerializeField] private GameObject text25;
    [SerializeField] private GameObject text35;

    private float Timer = 60.0f;
    [SerializeField] private TextMeshProUGUI textTimer;

    private bool oxygenBottle => savedDatas.oxygenBottle;

    [SerializeField] private GameObject textFin;
    
    // Nouvelles variables pour gérer les dégâts des ennemis IA
    [Header("Combat System")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private int blinkCount = 5;
    [SerializeField] private AudioClip enemyHitSound;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Start()
    {
        gameOver.SetActive(false);
        textFin.SetActive(false);
        // Initialiser les nouveaux composants
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (IsOnShop())
        {
            textShop.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene("scene_shop");
            }
        }
        else
        {
            textShop.SetActive(false);
        }

        if (Hp <= 0)
        {
            Dead();
        }

        if (IsOnFlagNext())
        {
            textFlagNext.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                switch (CurrentLevel)
                {
                    case 0.5:
                        SceneManager.LoadScene("Level 1");
                        savedDatas.CurrentLevel += 0.5;
                        break;
                    case 1.5:
                        SceneManager.LoadScene("Level 2");
                        savedDatas.CurrentLevel += 0.5;
                        break;
                    case 2.5:
                        SceneManager.LoadScene("Level 3");
                        savedDatas.CurrentLevel += 0.5;
                        break;
                    case 3.5:
                        if (oxygenBottle)
                        {
                            SceneManager.LoadScene("Level 4");
                            savedDatas.CurrentLevel += 0.5;
                        }
                        break;
                    case 4:
                        SceneManager.LoadScene("Fin");
                        savedDatas.CurrentLevel += 0.5;
                        break;
                    default:
                        SceneManager.LoadScene("scene_camp_de_base");
                        savedDatas.CurrentLevel += 0.5;
                        break;

                }
            }
        }
        else
        {
            textFlagNext.SetActive(false);
        }
        switch (CurrentLevel)
        {
            case 0.5:
                text05.SetActive(true);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
            case 1.5:
                text05.SetActive(false);
                text15.SetActive(true);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
            case 2.5:
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(true);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
            case 3.5:
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(true);
                textFin.SetActive(false);
                break;
            case 4.5:
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(true);
                break;
            default:
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
        }
        if (CurrentLevel == 4)
        {
            textTimer.enabled = true;
            Timer -= Time.deltaTime;
            textTimer.text = Timer.ToString();
            if (Timer <= 0.0f)
            {
                Dead();
            }
        }
        else
        {
            textTimer.enabled = false;
        }
    }

    private bool IsOnShop()
    {
        return Physics2D.OverlapCircle(collisionCheck.position, 0.2f, shopLayer);
    }
    private bool IsOnFlagNext()
    {
        return Physics2D.OverlapCircle(collisionCheck.position, 0.2f, flagLayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coins"))
        {
            savedDatas.Money += coinValue;
            Destroy(other.gameObject);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Garder votre système existant pour les ennemis simples
            TakeDamageFromEnemy(damages);
        }
    }
    
    // Nouvelle méthode publique pour que les ennemis IA puissent infliger des dégâts
    public void TakeDamageFromEnemy(int damage)
    {
        // Si le joueur est invincible, ne pas prendre de dégâts
        if (isInvincible)
            return;
            
        // Réduire la vie
        savedDatas.Hp -= damage;
        
        // Jouer les effets visuels et sonores
        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }
        
        if (enemyHitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(enemyHitSound);
        }
        
        // Appliquer l'effet de knockback (optionnel)
        ApplyKnockback();
        
        // Vérifier si le joueur est mort
        if (savedDatas.Hp <= 0)
        {
            Dead();
        }
        else
        {
            // Période d'invincibilité temporaire
            StartCoroutine(BecomeInvincible());
        }
    }
    
    private void ApplyKnockback()
    {
        // Appliquer une force de recul léger
        if (rb != null)
        {
            // Déterminer la direction du knockback
            Vector2 knockbackDirection = Vector2.zero;
            
            // Trouver l'ennemi le plus proche
            WolfEnemyAI[] wolves = FindObjectsOfType<WolfEnemyAI>();
            float closestDistance = float.MaxValue;
            Vector2 playerPosition = transform.position;
            
            foreach (WolfEnemyAI wolf in wolves)
            {
                float distance = Vector2.Distance(playerPosition, wolf.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    knockbackDirection = (playerPosition - (Vector2)wolf.transform.position).normalized;
                }
            }
            
            // Si aucun loup n'est trouvé, utiliser une direction par défaut
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
            
            // Appliquer la force de knockback
            rb.AddForce(knockbackDirection * 3f, ForceMode2D.Impulse);
        }
    }
    
    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        
        // Effet de clignotement pour montrer l'invincibilité
        if (spriteRenderer != null)
        {
            for (int i = 0; i < blinkCount; i++)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                yield return new WaitForSeconds(invincibilityDuration / (blinkCount * 2));
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(invincibilityDuration / (blinkCount * 2));
            }
        }
        else
        {
            // Si pas de SpriteRenderer, juste attendre
            yield return new WaitForSeconds(invincibilityDuration);
        }
        
        isInvincible = false;
    }
    
    private void Dead()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        gameOver.SetActive(true);
        savedDatas.Hp = 100;
        savedDatas.CurrentLevel -= 0.5;
    }
    
    // Méthodes utilitaires publiques
    public int GetCurrentHp()
    {
        return savedDatas.Hp;
    }
    
    public bool IsInvincible()
    {
        return isInvincible;
    }
}