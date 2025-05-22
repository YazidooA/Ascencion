using UnityEngine;

public class camera_scale : MonoBehaviour
{
    private int unitsToShowHorizontally = 18;
    private int unitsToShowVertically = 13;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float screenWidth = unitsToShowHorizontally;

        float screenHeight = unitsToShowVertically;

        float orthographicSize = screenHeight / 2f;

        Camera.main.orthographicSize = orthographicSize;

        Camera.main.aspect = screenWidth / screenHeight;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
