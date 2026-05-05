namespace Managment.Pooling
{
    public interface IPoolable
    {
        void OnTakenFromPool();
        void OnReturnedToPool();
    }
}