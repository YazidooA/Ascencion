using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBackToScene : MonoBehaviour
{
    public void GoBackToScene()
    {
        SceneManager.LoadScene(1);
        Debug.Log("pressed");
    }
}
