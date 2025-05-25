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

    #region Seeding mock data
    public void SeedingMockData()
    {
        SeedingOrdersMockData();

        SeedingClientsMockData();

        SeedingServiceMockData();

        SeedingGroupMockData();

        SeedItemsForOrderGroups();
    }

    private void SeedingOrdersMockData()
    {
        List<Order> orders =
        [
            new Order { Id = 1, Name = "طلبيه  1 ", ClientId = 1, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 2, Name = "طلبيه  2 ", ClientId = 2, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 3, Name = "طلبيه  3 ", ClientId = 3, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 4, Name = "طلبيه  4 ", ClientId = 4, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 5, Name = "طلبيه  5 ", ClientId = 5, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 6, Name = "طلبيه  6 ", ClientId = 6, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 7, Name = "طلبيه  7 ", ClientId = 7, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 8, Name = "طلبيه 10 ", ClientId = 8, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 9, Name = "طلبيه 11 ", ClientId = 9, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 10, Name = "طلبيه 21 ", ClientId = 10, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 11, Name = "طلبيه 31 ", ClientId = 11, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 12, Name = "طلبيه 41 ", ClientId = 12, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 13, Name = "طلبيه 51 ", ClientId = 13, TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = 14, Name = "طلبيه 61 ", ClientId = 14, TotalPrice = 500, TotalPaid = 50 },
        ];


        foreach (var order in orders)
        {
            // Check if the order already exists in the database
            if (!_dbContext.Order.Any(o => o.Id == order.Id))
            {
                _dbContext.Order.Add(order);
                Console.WriteLine(string.Format("new order add with id {0}", order.Id));
            }
        }
    }

    private void SeedingGroupMockData()
    {
        List<OrderGroup> orderGroup =
        [
        new OrderGroup { Id = 1, Name = "مجموعة 1", OrderId = 1 },
        new OrderGroup { Id = 2, Name = "مجموعة 2", OrderId = 1 },
        new OrderGroup { Id = 3, Name = "مجموعة 3", OrderId = 1 },
        new OrderGroup { Id = 4, Name = "مجموعة 4", OrderId = 1 },

        new OrderGroup { Id = 5, Name = "مجموعة 1", OrderId = 2 },
        new OrderGroup { Id = 6, Name = "مجموعة 2", OrderId = 2 },
        new OrderGroup { Id = 7, Name = "مجموعة 3", OrderId = 2 },
        new OrderGroup { Id = 8, Name = "مجموعة 4", OrderId = 2 },

        new OrderGroup { Id = 9, Name = "مجموعة  1", OrderId = 3 },
        new OrderGroup { Id = 10, Name = "مجموعة 2", OrderId = 3 },
        new OrderGroup { Id = 11, Name = "مجموعة 3", OrderId = 3 },
        new OrderGroup { Id = 12, Name = "مجموعة 4", OrderId = 3 },

        new OrderGroup { Id = 13, Name = "مجموعة 1", OrderId = 4 },
        new OrderGroup { Id = 14, Name = "مجموعة 2", OrderId = 4 },
        new OrderGroup { Id = 15, Name = "مجموعة 3", OrderId = 4 },
        new OrderGroup { Id = 16, Name = "مجموعة 4", OrderId = 4 },

        new OrderGroup { Id = 17, Name = "مجموعة 1", OrderId = 5 },
        new OrderGroup { Id = 18, Name = "مجموعة 2", OrderId = 5 },
        new OrderGroup { Id = 19, Name = "مجموعة 3", OrderId = 5 },
        new OrderGroup { Id = 20, Name = "مجموعة 4", OrderId = 5 },

        new OrderGroup { Id = 21, Name = "مجموعة 1", OrderId = 6 },
        new OrderGroup { Id = 22, Name = "مجموعة 2", OrderId = 6 },
        new OrderGroup { Id = 23, Name = "مجموعة 3", OrderId = 6 },
        new OrderGroup { Id = 24, Name = "مجموعة 4", OrderId = 6 },

        new OrderGroup { Id = 25, Name = "مجموعة 1", OrderId = 7 },
        new OrderGroup { Id = 26, Name = "مجموعة 2", OrderId = 7 },
        new OrderGroup { Id = 27, Name = "مجموعة 3", OrderId = 7 },
        new OrderGroup { Id = 28, Name = "مجموعة 4", OrderId = 7 },

        new OrderGroup { Id = 29, Name = "مجموعة 1", OrderId = 8 },
        new OrderGroup { Id = 30, Name = "مجموعة 2", OrderId = 8 },
        new OrderGroup { Id = 31, Name = "مجموعة 3", OrderId = 8 },
        new OrderGroup { Id = 32, Name = "مجموعة 4", OrderId = 8 },

        new OrderGroup { Id = 33, Name = "مجموعة 1", OrderId = 9 },
        new OrderGroup { Id = 34, Name = "مجموعة 2", OrderId = 10 },
        new OrderGroup { Id = 35, Name = "مجموعة 3", OrderId = 11 },
        new OrderGroup { Id = 36, Name = "مجموعة 4", OrderId = 12 },
        new OrderGroup { Id = 37, Name = "مجموعة 4", OrderId = 13 },
        new OrderGroup { Id = 38, Name = "مجموعة 4", OrderId = 14 },



    ];

        foreach (var group in orderGroup)
        {
            // Check if the order already exists in the database
            if (!_dbContext.OrderGroup.Any(o => o.Id == group.Id))
            {
                _dbContext.OrderGroup.Add(group);
                Console.WriteLine(string.Format("order group add {0} for order {1}", group.Name, group.OrderId));
            }
        }
    }

    private void SeedItemsForOrderGroups()
    {
        // Retrieve all existing OrderGroups from the database
        var orderGroups = _dbContext.OrderGroup.ToList();

        // Create a list to hold new items
        List<Item> itemsToAdd = new();

        var itemId = 1;

        foreach (var group in orderGroups)
        {
            // Define Arabic item names
            var arabicItemNames = new[]
            {
            "كناب 1",
            "كناب 2",
            "كناب 3",
            "كناب 4",
            "كناب 5"
        };
            // Create five items for each order group
            for (int i = 0; i < arabicItemNames.Length; i++)
            {
                var newItem = new Item
                {
                    Id = itemId++, // Assuming it's auto-generated by the database
                    Name = arabicItemNames[i],
                    OrderGroupId = group.Id,
                    Quantity = (i + 1) * 2, // Example: 2, 4, 6, 8, 10
                    Price = (i + 1) * 10.5m // Example: 10.5, 21, 31.5, etc.
                };

                itemsToAdd.Add(newItem);
            }
        }

        foreach (var item in itemsToAdd)
        {
            // Check if the client already exists in the database
            if (!_dbContext.Item.Any(c => c.Id == item.Id))
            {
                _dbContext.Item.Add(item);
                Console.WriteLine(string.Format("new item add with id {0}", item.Id));

            }
        }


    }

    private void SeedingClientsMockData()
    {
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
                Console.WriteLine(string.Format("new client add with id {0}", client.Id));

            }
        }


    }


    private void SeedingServiceMockData()
    {
        List<Service> services =
        [
           new Service { Id = 1, Name = "طباعة ورق 70 جم الوان", Price = 0.5m, ServiceCategory = ServiceCategoryEnum.Printing },
           new Service { Id = 2, Name = "طباعة ورق 80 جم الوان", Price = 0.7m, ServiceCategory = ServiceCategoryEnum.Printing },
           new Service { Id = 3, Name = "قص", Price = 0.5m, ServiceCategory = ServiceCategoryEnum.Cutting },
           new Service { Id = 4, Name = "لصق", Price = 0.5m, ServiceCategory = ServiceCategoryEnum.Clueing },
           new Service { Id = 5, Name = "تدبيس", Price = 0.5m, ServiceCategory = ServiceCategoryEnum.Stapling },
           new Service { Id = 6, Name = "بيع", Price = 0.5m, ServiceCategory = ServiceCategoryEnum.Selling },
        ];


        foreach (var service in services)
        {
            // Check if the order already exists in the database
            if (!_dbContext.Service.Any(o => o.Id == service.Id))
            {
                _dbContext.Service.Add(service);
                Console.WriteLine(string.Format("new service add with id {0}", service.Id));
            }
        }
    }

    #endregion


    public void SeedingLockupData()
    {
        SeedingItemDetailsKeyLKP();
        SeedingServiceCategoryLKP();
    }

    private void SeedingItemDetailsKeyLKP()
    {
        foreach (var itemDetailsKey in Enum.GetValues(typeof(ItemDetailsKeyEnum)))
        {
            if (!_dbContext.ItemDetailsKey_LKP.Any(i => i.Id == (int)itemDetailsKey))
            {
                _dbContext.ItemDetailsKey_LKP.Add(new ItemDetailsKey_LKP { Id = (int)itemDetailsKey, Name = itemDetailsKey.ToString() });

            }
        }
    }

    private void SeedingServiceCategoryLKP()
    {
        foreach (var serviceCategory in Enum.GetValues(typeof(ServiceCategoryEnum)))
        {
            if (!_dbContext.ServiceCategory_LKP.Any(i => i.Id == (int)serviceCategory))
            {
                _dbContext.ServiceCategory_LKP.Add(new ServiceCategory_LKP { Id = (int)serviceCategory, Name = serviceCategory.ToString() });
            }
        }
    }
}
