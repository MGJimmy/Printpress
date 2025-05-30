﻿using Printpress.Application;
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

        private IGenericRepository<Service> _serviceRepository;
        private IGenericRepository<Order> _orderRepository;
        private IGenericRepository<Client> _clientRepository;
        private IGenericRepository<OrderTransaction> _orderTransactionRepository;
        private IGenericRepository<ItemDetails> _itemDetailsRepository;
        private IGenericRepository<OrderGroup> _orderGroupRepository;






        public IGenericRepository<Service> ServiceRepository
        {
            get
            {
                if (_serviceRepository == null)
                {
                    _serviceRepository = new GenericRepository<Service>(_context);
                }

                return _serviceRepository;
            }
        }
        public IGenericRepository<ItemDetails> ItemDetailsRepository
        {
            get
            {
                if (_itemDetailsRepository == null)
                {
                    _itemDetailsRepository = new GenericRepository<ItemDetails>(_context);
                }

                return _itemDetailsRepository;
            }
        }
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

        public IGenericRepository<OrderGroup> OrderGroupRepository
        {
            get
            {
                if (_orderGroupRepository == null)
                {
                    _orderGroupRepository = new GenericRepository<OrderGroup>(_context);
                }
                return _orderGroupRepository;
            }
        }

        public async Task SaveChangesAsync(string userId)
        {
            _context.CurrentUserId = userId;
            await _context.SaveChangesAsync();
        }
    }
}
