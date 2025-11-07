namespace Loterias.Shared.Interfaces
{
    public interface ISharedLogger
    {
        void Info(string message, object? details = null);
        void Warn(string message, object? details = null);
        void Error(string message, Exception ex, object? details = null);
    }
}
