using System;
using System.Collections;
using System.Collections.Generic;

/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/12/2025
 * UPDATED: 01/12/2025 | BY: Chase Morgan  | COMMENTS: Added class
 * FILE DESCRIPTION: Holds data for strategy enumerators
 */

namespace ChaseMorgan.Strategy
{
    public enum StrategyEnum
    {
        //Move,
        SlamLeft,
        SlamRight,
        BigSlam,
        SwipeLeft,
        SwipeRight,
        Roll,
        Airstrike,
        RockLift,
        Charge,
        Spin,
        Sandstorm
    }

    public static class StrategyEnumerator
    {
        public static readonly IDictionary<StrategyEnum, Type> strategies = new Dictionary<StrategyEnum, Type>()
        {
            //{ StrategyEnum.Move, typeof(MoveStrategy) },
            { StrategyEnum.SlamLeft, typeof(SlamLeftStrategy) },
            { StrategyEnum.SlamRight, typeof(SlamRightStrategy) },
            { StrategyEnum.BigSlam, typeof(BigSlamStrategy) },
            { StrategyEnum.SwipeLeft, typeof(SwipeLeftStrategy) },
            { StrategyEnum.SwipeRight, typeof(SwipeRightStrategy) },
            { StrategyEnum.Roll, typeof(RollStrategy) },
            { StrategyEnum.Airstrike, typeof(AirstrikeStrategy) },
            { StrategyEnum.RockLift, typeof(RockLiftStrategy) },
            { StrategyEnum.Charge, typeof(ChargeStrategy) },
            { StrategyEnum.Spin, typeof(SpinStrategy) },
            { StrategyEnum.Sandstorm, typeof(SandstormStrategy) },
        };
    }
}
