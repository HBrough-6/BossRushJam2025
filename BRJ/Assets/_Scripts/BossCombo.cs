using ChaseMorgan.Strategy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/12/2025
 * UPDATED: 01/12/2025 | BY: Chase Morgan | COMMENTS: Added class
 * FILE DESCRIPTION: BossCombo class to hold data for combos
 */

[CreateAssetMenu(fileName = "NewBossCombo", menuName = "Boss/New Combo")]
public class BossCombo : ScriptableObject
{
    public StrategyEnum[] combo;
    [Tooltip("The timings in between each attack (leave unitialized for no time inbetween attacks)")]
    public float[] timings;
    public bool canBeCancelled = true;
}
