using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossCombo", menuName = "Boss/New Combo")]
public class BossCombo : ScriptableObject
{
    public StrategyEnum[] combo;
    [Tooltip("The timings in between each attack (leave unitialized for no time inbetween attacks)")]
    public float[] timings;
    public bool canBeCancelled = true;
}
