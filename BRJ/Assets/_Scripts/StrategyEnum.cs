using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaseMorgan.Strategy
{
    public enum StrategyEnum
    {
        Move
    }

    public static class StrategyEnumerator
    {
        public static readonly IDictionary<StrategyEnum, Type> strategies = new Dictionary<StrategyEnum, Type>()
        {
            { StrategyEnum.Move, typeof(MoveStrategy) }
        };
    }
}
