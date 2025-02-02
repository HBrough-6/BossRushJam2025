using UnityEngine.Events;

namespace ChaseMorgan.Strategy
{
    /* FILE HEADER
     * AUTHOR: Chase Morgan | CREATED: 01/11/2025
     * UPDATED: 01/11/2025 | BY: Chase Morgan  | COMMENTS: Added interface/namespace
     * FILE DESCRIPTION: Strategy interface
     */
    public interface IStrategy
    {
        public StrategyMaxRange MaxRange { get; set; }
        public void Execute(Client client, UnityAction callback = null);
        public void Disable();
    }
}
