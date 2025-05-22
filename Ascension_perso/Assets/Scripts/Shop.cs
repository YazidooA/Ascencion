using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;
    [SerializeField] private GameObject redCross1;
    [SerializeField] private GameObject redCross2;
    [SerializeField] private GameObject redCross3;
    [SerializeField] private GameObject redCross4;
    private int Money => savedDatas.Money;
    private int Hp => savedDatas.Hp;
    [SerializeField] private int bootsPrice = 50;
    [SerializeField] private int dashPrice = 100;
    [SerializeField] private int heartPrice = 10;
    [SerializeField] private int oxygenPrice = 150;



    public void GoToBaseCamp()
    {
        SceneManager.LoadScene("scene_camp_de_base");
    }
    public void BuyButton1()
    {
        if (Money >= bootsPrice)
        {
            if (!savedDatas.allowWallSticking)
            {
                savedDatas.allowWallSticking = true;
                savedDatas.Money -= bootsPrice;
            }
        }

    }
    public void BuyButton2()
    {
        if (Money >= dashPrice)
        {
            if (!savedDatas.allowDash)
            {
                savedDatas.allowDash = true;
                savedDatas.Money -= dashPrice;
            }
        }
    }
    public void BuyButton4()
    {
        if (Money >= heartPrice)
        {
            if (Hp != 100)
            {
                savedDatas.Hp = 100;
            }
        }
    }

    public void BuyButton3()
    {
        if (Money >= oxygenPrice)
        {
            if (!savedDatas.oxygenBottle)
            {
                savedDatas.oxygenBottle = true;
                savedDatas.Money -= oxygenPrice;
            }
        }
    }

    void Update()
    {
        if (savedDatas.allowWallSticking)
        {
            redCross1.SetActive(true);
        }
        else
        {
            redCross1.SetActive(false);
        }

        if (savedDatas.allowDash)
        {
            redCross2.SetActive(true);
        }
        else
        {
            redCross2.SetActive(false);
        }

        if (Hp == 100)
        {
            redCross4.SetActive(true);
        }
        else
        {
            redCross4.SetActive(false);
        }

        if (savedDatas.oxygenBottle)
        {
            redCross3.SetActive(true);
        }
        else
        {
            redCross3.SetActive(false);
        }
    }
}
