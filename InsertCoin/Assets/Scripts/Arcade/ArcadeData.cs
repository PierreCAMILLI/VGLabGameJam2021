using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArcadeData", menuName = "Arcade/Data", order = 1)]
public class ArcadeData : ScriptableObject
{
    public int Score { get; set; }
    public int Credits { get; set; }
    public float ContinueTimer { get; set; }
}
