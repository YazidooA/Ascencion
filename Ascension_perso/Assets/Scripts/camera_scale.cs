using UnityEngine;

public class camera_scale : MonoBehaviour
{
    [SerializeField] private int unitsToShowHorizontally = 16;
    [SerializeField] private int unitsToShowVertically = 9;

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
