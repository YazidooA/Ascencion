using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class DisplayPlayerInfos : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;
    [SerializeField] private TextMeshProUGUI MoneyText;
    [SerializeField] private TextMeshProUGUI HpText;
    private int Money => savedDatas.Money;
    private int Hp => savedDatas.Hp;
    private float Timer => savedDatas.Timer;
    [SerializeField] private TextMeshProUGUI textTimer;

    void Update()
    {
        MoneyText.text = Money.ToString();
        HpText.text = Hp.ToString();
        textTimer.text = Timer.ToString();
    }
    public void BackToBaseCamp()
    {
        SceneManager.LoadScene("scene_camp_de_base");
    }
}
