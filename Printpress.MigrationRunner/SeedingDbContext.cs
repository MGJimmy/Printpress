using Printpress.Domain.Entities;
using Printpress.Domain.Enums;
using Printpress.Infrastructure;

namespace Printpress.MigrationRunner;

internal sealed class SeedingDbContext
{

    private readonly ApplicationDbContext _dbContext;

    public SeedingDbContext(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void SeedingMockData()
    {
        List<Order> orders =
        [
            new Order { Id = 1, Name = "طلبيه  1 ", ClientId = 1, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 2, Name = "طلبيه  2 ", ClientId = 2, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 3, Name = "طلبيه  3 ", ClientId = 3, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 4, Name = "طلبيه  4 ", ClientId = 4, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 5, Name = "طلبيه  5 ", ClientId = 5, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 6, Name = "طلبيه  6 ", ClientId = 6, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 7, Name = "طلبيه  7 ", ClientId = 7, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 8, Name = "طلبيه 10 ", ClientId = 8, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 9, Name = "طلبيه 11 ", ClientId = 9, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 10, Name = "طلبيه 21 ", ClientId = 10, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 11, Name = "طلبيه 31 ", ClientId = 11, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 12, Name = "طلبيه 41 ", ClientId = 12, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 13, Name = "طلبيه 51 ", ClientId = 13, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 14, Name = "طلبيه 61 ", ClientId = 14, Status = OrderStatusEnum.New, TotalPrice = 500, TotalPaid = 50 },
        ];


        foreach (var order in orders)
        {
            // Check if the order already exists in the database
            if (!_dbContext.Order.Any(o => o.Id == order.Id))
            {
                _dbContext.Order.Add(order);
            }
        }

        List<Client> clients =
        [
            new Client { Id = 1, Name = "عميل 1", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 2, Name = "عميل 2", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 3, Name = "عميل 3", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 4, Name = "عميل 4", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 5, Name = "عميل 5", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 6, Name = "عميل 6", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 7, Name = "عميل 7", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 8, Name = "عميل 8", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 9, Name = "عميل 9", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 10, Name = "عميل 10", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 11, Name = "عميل 11", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 12, Name = "عميل 12", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 13, Name = "عميل 13", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 14, Name = "عميل 14", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = 15, Name = "عميل 15", Mobile = "0111284858", Address = "first streat 488" },
        ];


        foreach (var client in clients)
        {
            // Check if the client already exists in the database
            if (!_dbContext.Client.Any(c => c.Id == client.Id))
            {
                _dbContext.Client.Add(client);
            }
        }

    }



    public void SeedingLockupData()
    {
        //shluld add the lockup data here

    }
}
