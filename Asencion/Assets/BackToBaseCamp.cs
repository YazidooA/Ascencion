using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToBaseCamp : MonoBehaviour
{
    public void GoToBaseCamp()
    {
        SceneManager.LoadScene("Scene_Camps_de_bases");
    }
}
