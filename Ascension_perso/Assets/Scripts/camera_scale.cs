using UnityEngine;

public class camera_scale : MonoBehaviour
{
    private int unitsToShowHorizontally = 22;
    private int unitsToShowVertically = 16;
    void Start()
    {
        float screenWidth = unitsToShowHorizontally;

        float screenHeight = unitsToShowVertically;

        float orthographicSize = screenHeight / 2f;

        Camera.main.orthographicSize = orthographicSize;

        Camera.main.aspect = screenWidth / screenHeight;
    }
}
