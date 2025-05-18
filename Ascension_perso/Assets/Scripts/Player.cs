using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Player : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;
    private string _name;
    
    
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
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            savedDatas.Hp -= damages;
        }
    }

}