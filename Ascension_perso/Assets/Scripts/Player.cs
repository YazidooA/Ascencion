using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;

    [SerializeField] private GameObject text;
    [SerializeField] private Transform collisionCheck;
    [SerializeField] private LayerMask shopLayer;

    private int Money => savedDatas.Money;
    [SerializeField] private int coinValue = 1;

    private int Hp => savedDatas.Hp;
    [SerializeField] private int damages = 20;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject gameOver;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private ParticleSystem bloodParticles;

    private void Start()
    {
        gameOver.SetActive(false); ;
    }

    void Update()
    {
        if (IsOnShop())
        {
            text.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene("scene_shop");
            }
        }
        else
        {
            text.SetActive(false);
        }
        if (Hp == 0)
        {
            Dead();
        }

    }

    private bool IsOnShop()
    {
        return Physics2D.OverlapCircle(collisionCheck.position, 0.2f, shopLayer);
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
    }
}
