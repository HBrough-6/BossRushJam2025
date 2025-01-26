using System;
using System.Collections;
using System.Collections.Generic;

/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/12/2025
 * UPDATED: 01/19/2025 | BY: Chase Morgan  | COMMENTS: updated enumerator
 * FILE DESCRIPTION: Holds data for strategy enumerators
 */

namespace ChaseMorgan.Strategy
{
    //📋 = in progress
    //✅ = finished
    public enum StrategyEnum
    {
        //Move, //Shouldn't be used as a strategy (the boss classes will handle it themselves)
        SlamLeft, //✅
        SlamRight, //✅
        BigSlam, //✅
        SwipeLeft, //✅
        SwipeRight, //✅
        Roll, //✅
        Airstrike, //✅
        RockLift, //✅
        Leap, //📋
        Spin, //📋
        Sandstorm //✅
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
            { StrategyEnum.Leap, typeof(ChargeStrategy) },
            { StrategyEnum.Spin, typeof(SpinStrategy) },
            { StrategyEnum.Sandstorm, typeof(SandstormStrategy) },
        };
    }
}
