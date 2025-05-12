using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;
    [SerializeField] private GameObject redCross1;
    [SerializeField] private GameObject redCross2;
    private int Money => savedDatas.Money;
    [SerializeField] private int bootsPrice = 50;
    [SerializeField] private int dashPrice = 100;



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
    }
}
