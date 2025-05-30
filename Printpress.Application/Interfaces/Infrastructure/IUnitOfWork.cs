using Printpress.Domain.Entities;

namespace Printpress.Application;

public interface IUnitOfWork
{
    IGenericRepository<Service> ServiceRepository { get; }
    IGenericRepository<Client> ClientRepository { get; }
    IGenericRepository<Order> OrderRepository { get; }
    IGenericRepository<OrderTransaction> OrderTransactionRepository { get; }
    IGenericRepository<ItemDetails> ItemDetailsRepository { get; }

    IGenericRepository<OrderGroup> OrderGroupRepository { get; }

    Task SaveChangesAsync(string userId);
}
