using UnityEngine;


[CreateAssetMenu(fileName = "storeData", menuName = "storeData", order = 0)]
public class storeData : ScriptableObject
{
    public bool allowDash;
    public bool allowWallSticking;
    public bool oxygenBottle;
    public int Money;
    public int Hp;
    public double CurrentLevel;
    public float Timer;
}
