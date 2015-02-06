namespace Shinkei.IRC.Events
{
    public interface ICancellable
    {
        bool IsCancelled();
        void SetCancelled(bool cancel);
    }
}