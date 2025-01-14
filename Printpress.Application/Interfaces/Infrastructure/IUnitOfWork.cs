﻿using Printpress.Domain.Entities;

namespace Printpress.Application;

public interface IUnitOfWork
{
    IGenericRepository<Client> ClientRepository { get; }
    IGenericRepository<Order> OrderRepository { get; }

    IGenericRepository<OrderTransaction> OrderTransactionRepository { get; }


    Task SaveChangesAsync();
}
