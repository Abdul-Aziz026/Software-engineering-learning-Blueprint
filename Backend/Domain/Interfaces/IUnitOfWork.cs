
namespace Domain.Interfaces;

// Generic Unit of Work interface
public interface IUnitOfWork : IDisposable
{
    Task StartTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    void Dispose();
}
