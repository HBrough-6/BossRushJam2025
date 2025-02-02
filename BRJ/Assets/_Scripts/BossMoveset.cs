using ChaseMorgan.Strategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/12/2025
 * UPDATED: 01/12/2025 | BY: Chase Morgan  | COMMENTS: Added class
 * FILE DESCRIPTION: BossMoveset class to hold data for a bosses' moveset
 */

[CreateAssetMenu(fileName = "NewBossMoveset", menuName = "Boss/New Moveset")]
public class BossMoveset : ScriptableObject
{
    public BossCombo[] combos;
    public StrategyEnum[] attacks;

    public BossState activePhase = BossState.PhaseOne;
}
