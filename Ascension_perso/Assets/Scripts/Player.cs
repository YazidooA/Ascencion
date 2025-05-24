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

    private float Timer => savedDatas.Timer;
    [SerializeField] private GameObject textTimer;

    private bool oxygenBottle => savedDatas.oxygenBottle;

    [SerializeField] private GameObject textFin;
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
                textTimer.SetActive(false);
                text05.SetActive(true);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
            case 1.5:
                textTimer.SetActive(false);
                text05.SetActive(false);
                text15.SetActive(true);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
            case 2.5:
                textTimer.SetActive(false);
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(true);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
            case 3.5:
                textTimer.SetActive(false);
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(true);
                textFin.SetActive(false);
                break;
            case 4:
                textTimer.SetActive(false);
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(false);
                savedDatas.Timer -= Time.deltaTime;
                if (Timer <= 0.0f) Dead();
                break;
            case 4.5:
                textTimer.SetActive(false);
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(true);
                break;
            default:
                textTimer.SetActive(false);
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
                textFin.SetActive(false);
                break;
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
        if (other.gameObject.CompareTag("Enemy")) TakeDamageFromEnemy(damages);
    }
    public void TakeDamageFromEnemy(int damage)
    {
        if (isInvincible) return;
        savedDatas.Hp -= damage;
        if (bloodParticles != null) bloodParticles.Play();
        if (enemyHitSound != null && audioSource != null) audioSource.PlayOneShot(enemyHitSound);
        ApplyKnockback();
        if (savedDatas.Hp <= 0) Dead();
        else StartCoroutine(BecomeInvincible());
    }
    private void ApplyKnockback()
    {
        if (rb != null)
        {
            Vector2 knockbackDirection = Vector2.zero;
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
            if (knockbackDirection == Vector2.zero) knockbackDirection = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
            rb.AddForce(knockbackDirection * 3f, ForceMode2D.Impulse);
        }
    }
    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
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
        else yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
    
    public void Dead()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        gameOver.SetActive(true);
        savedDatas.Hp = 100;
        savedDatas.CurrentLevel -= 0.5;
    }
    public int GetCurrentHp()
    {
        return savedDatas.Hp;
    }
    
    public bool IsInvincible()
    {
        return isInvincible;
    }
}