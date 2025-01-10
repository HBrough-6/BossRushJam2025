namespace ChaseMorgan.Strategy
{
    public interface IStrategy
    {
        public void Execute(Client client);
        public void Disable();
    }
}
