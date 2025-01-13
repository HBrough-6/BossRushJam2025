using UnityEngine.Events;

namespace ChaseMorgan.Strategy
{
    public interface IStrategy
    {
        public void Execute(Client client, UnityAction callback = null);
        public void Disable();
    }
}
