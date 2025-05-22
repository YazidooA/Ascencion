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

    private void Start()
    {
        gameOver.SetActive(false); ;
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

        if (Hp == 0)
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
                break;
            case 1.5:
                text05.SetActive(false);
                text15.SetActive(true);
                text25.SetActive(false);
                text35.SetActive(false);
                break;
            case 2.5:
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(true);
                text35.SetActive(false);
                break;
            case 3.5:
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(true);
                break;
            default:
                text05.SetActive(false);
                text15.SetActive(false);
                text25.SetActive(false);
                text35.SetActive(false);
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
            savedDatas.Hp -= damages;
            bloodParticles.Play();
        }
    }
    private void Dead()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        gameOver.SetActive(true);
        savedDatas.Hp = 100;
        savedDatas.CurrentLevel -= 0.5;
    }
}
