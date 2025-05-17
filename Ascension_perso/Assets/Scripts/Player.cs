using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;
    private string name;

    public string Name { get => name; set => name = value; }
    [SerializeField] private GameObject text;
    [SerializeField] private Transform? collisionCheck;
    [SerializeField] private LayerMask shopLayer;

    private int Money => savedDatas.Money;
    [SerializeField] private int coinValue = 1;

    private int Hp => savedDatas.Hp;
    [SerializeField] private int damages = 20;
    [SerializeField] private LayerMask enemyLayer;



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
        }
    }

}
