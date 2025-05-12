using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DisplayPlayerInfos : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;
    [SerializeField] private TextMeshProUGUI MoneyText;
    [SerializeField] private TextMeshProUGUI HpText;
    private int Money => savedDatas.Money;
    private int Hp => savedDatas.Hp = 100;


    void Update()
    {
        MoneyText.text = Money.ToString();
        HpText.text = Hp.ToString();
    }
}
