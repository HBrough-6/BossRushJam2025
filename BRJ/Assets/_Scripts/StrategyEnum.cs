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
