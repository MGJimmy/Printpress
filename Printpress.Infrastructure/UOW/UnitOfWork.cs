using Printpress.Application;
using Printpress.Domain.Entities;
using Printpress.Infrastructure.Repository;

namespace Printpress.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


        private IGenericRepository<Order> _orderRepository;

        private IGenericRepository<Client> _clientRepository;
        private IGenericRepository<OrderTransaction> _orderTransactionRepository;

        public IGenericRepository<Client> ClientRepository
        {
            get
            {
                if (_clientRepository == null)
                {
                    _clientRepository = new GenericRepository<Client>(_context);
                }

                return _clientRepository;
            }
        }

        public IGenericRepository<Order> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new GenericRepository<Order>(_context);
                }

                return _orderRepository;
            }
        }

        public IGenericRepository<OrderTransaction> OrderTransactionRepository
        {
            get
            {
                if (_orderTransactionRepository == null)
                {
                    _orderTransactionRepository = new GenericRepository<OrderTransaction>(_context);
                }

                return _orderTransactionRepository;
            }
        }



        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
