using Printpress.Application;
using Printpress.Domain.Entities;
using Printpress.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Infrastructure
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


        private IGenericRepository<Order> _orderRepository;





        public IGenericRepository<Order> OrderRepository { 
            get 
            {
                if(_orderRepository == null)
                {
                    _orderRepository = new GenericRepository<Order>(_context);
                }

                return _orderRepository;
            } 
        }
    }
}
