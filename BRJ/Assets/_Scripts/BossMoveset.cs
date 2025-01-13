using ChaseMorgan.Strategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossMoveset", menuName = "Boss/New Moveset")]
public class BossMoveset : ScriptableObject
{
    public BossCombo[] combos;
    public StrategyEnum[] attacks;

    public BossState activePhase = BossState.PhaseOne;
}
