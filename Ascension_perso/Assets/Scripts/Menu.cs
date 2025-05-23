using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private storeData savedDatas;

    public void BackButton()
    {
        SceneManager.LoadScene("Menu");
    }
    public void ControlsButton()
    {
        SceneManager.LoadScene("Menu Contrôles");
    }
    public void PlayButton()
    {
        SceneManager.LoadScene("Lobby");
        savedDatas.Hp = 100;
        savedDatas.Money = 0;
        savedDatas.CurrentLevel = 0.5;
        savedDatas.allowDash = false;
        savedDatas.allowWallSticking = false;
        savedDatas.oxygenBottle = false;
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
