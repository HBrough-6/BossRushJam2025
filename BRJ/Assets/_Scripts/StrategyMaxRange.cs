using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChaseMorgan.Strategy
{
    /// <summary>
    /// Max ranges for a strategy. These are defined by the client that uses them.
    /// </summary>
    public enum StrategyMaxRange
    {
        None,
        Small,
        Medium,
        Large,
        Custom
    }
}
