using Printpress.Application;
using Printpress.Domain;

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
        private IGenericRepository<ServiceCategory> _serviceCategoryRepository;
        private IGenericRepository<Order> _orderRepository;
        private IGenericRepository<Client> _clientRepository;
        private IGenericRepository<OrderTransaction> _orderTransactionRepository;
        private IGenericRepository<OrderItemDetails> _itemDetailsRepository;
        private IGenericRepository<OrderGroup> _orderGroupRepository;
        private IGenericRepository<OrderItem> _orderItemRepository;
        private IGenericRepository<OrderGroupService> _orderGroupServiceRepository;
        private IInventoryItemRepository _inventoryItemRepository;
        private IGenericRepository<InventoryTransaction> _inventoryTransactionRepository;
        private IGenericRepository<PurchaseInvoice> _purchaseInvoiceRepository;
        private IGenericRepository<PurchaseInvoiceLine> _purchaseInvoiceLineRepository;
        private ISparePartItemRepository _sparePartItemRepository;
        private IGenericRepository<SparePartInventoryTransaction> _sparePartTransactionRepository;
        private IGenericRepository<SparePartPurchaseInvoice> _sparePartPurchaseInvoiceRepository;
        private IGenericRepository<SparePartSellingInvoice> _sparePartSellingInvoiceRepository;
        private IReportRepository _reportRepository;
        private IGenericRepository<Worker> _workerRepository;
        private IGenericRepository<ItemServiceExecution> _workerProductionRepository;
        private IGenericRepository<WorkerSalaryTransaction> _workerSalaryTransactionRepository;
        private IGenericRepository<PayrollPeriod> _payrollPeriodRepository;




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

        public IGenericRepository<ServiceCategory> ServiceCategoryRepository
        {
            get
            {
                if (_serviceCategoryRepository == null)
                {
                    _serviceCategoryRepository = new GenericRepository<ServiceCategory>(_context);
                }

                return _serviceCategoryRepository;
            }
        }
        public IGenericRepository<OrderItemDetails> ItemDetailsRepository
        {
            get
            {
                if (_itemDetailsRepository == null)
                {
                    _itemDetailsRepository = new GenericRepository<OrderItemDetails>(_context);
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

        public IGenericRepository<OrderItem> OrderItemRepository
        {
            get
            {
                if (_orderItemRepository == null)
                    _orderItemRepository = new GenericRepository<OrderItem>(_context);
                return _orderItemRepository;
            }
        }

        public IGenericRepository<OrderGroupService> OrderGroupServiceRepository
        {
            get
            {
                if (_orderGroupServiceRepository == null)
                    _orderGroupServiceRepository = new GenericRepository<OrderGroupService>(_context);
                return _orderGroupServiceRepository;
            }
        }

        public IInventoryItemRepository InventoryItemRepository
        {
            get
            {
                if (_inventoryItemRepository == null)
                    _inventoryItemRepository = new InventoryItemRepository(_context);
                return _inventoryItemRepository;
            }
        }

        public IGenericRepository<InventoryTransaction> InventoryTransactionRepository
        {
            get
            {
                if (_inventoryTransactionRepository == null)
                    _inventoryTransactionRepository = new GenericRepository<InventoryTransaction>(_context);
                return _inventoryTransactionRepository;
            }
        }

        public IGenericRepository<PurchaseInvoice> PurchaseInvoiceRepository
        {
            get
            {
                if (_purchaseInvoiceRepository == null)
                    _purchaseInvoiceRepository = new GenericRepository<PurchaseInvoice>(_context);
                return _purchaseInvoiceRepository;
            }
        }

        public IGenericRepository<PurchaseInvoiceLine> PurchaseInvoiceLineRepository
        {
            get
            {
                if (_purchaseInvoiceLineRepository == null)
                    _purchaseInvoiceLineRepository = new GenericRepository<PurchaseInvoiceLine>(_context);
                return _purchaseInvoiceLineRepository;
            }
        }

        public ISparePartItemRepository SparePartItemRepository
        {
            get
            {
                if (_sparePartItemRepository == null)
                    _sparePartItemRepository = new SparePartItemRepository(_context);
                return _sparePartItemRepository;
            }
        }

        public IGenericRepository<SparePartInventoryTransaction> SparePartTransactionRepository
        {
            get
            {
                if (_sparePartTransactionRepository == null)
                    _sparePartTransactionRepository = new GenericRepository<SparePartInventoryTransaction>(_context);
                return _sparePartTransactionRepository;
            }
        }

        public IGenericRepository<SparePartPurchaseInvoice> SparePartPurchaseInvoiceRepository
        {
            get
            {
                if (_sparePartPurchaseInvoiceRepository == null)
                    _sparePartPurchaseInvoiceRepository = new GenericRepository<SparePartPurchaseInvoice>(_context);
                return _sparePartPurchaseInvoiceRepository;
            }
        }

        public IGenericRepository<SparePartSellingInvoice> SparePartSellingInvoiceRepository
        {
            get
            {
                if (_sparePartSellingInvoiceRepository == null)
                    _sparePartSellingInvoiceRepository = new GenericRepository<SparePartSellingInvoice>(_context);
                return _sparePartSellingInvoiceRepository;
            }
        }

        public IReportRepository ReportRepository
        {
            get
            {
                if (_reportRepository == null)
                    _reportRepository = new ReportRepository(_context);
                return _reportRepository;
            }
        }

        public IGenericRepository<Worker> WorkerRepository
        {
            get
            {
                if (_workerRepository == null)
                    _workerRepository = new GenericRepository<Worker>(_context);
                return _workerRepository;
            }
        }

        public IGenericRepository<ItemServiceExecution> WorkerProductionRepository
        {
            get
            {
                if (_workerProductionRepository == null)
                    _workerProductionRepository = new GenericRepository<ItemServiceExecution>(_context);
                return _workerProductionRepository;
            }
        }

        public IGenericRepository<WorkerSalaryTransaction> WorkerSalaryTransactionRepository
        {
            get
            {
                if (_workerSalaryTransactionRepository == null)
                    _workerSalaryTransactionRepository = new GenericRepository<WorkerSalaryTransaction>(_context);
                return _workerSalaryTransactionRepository;
            }
        }

        public IGenericRepository<PayrollPeriod> PayrollPeriodRepository
        {
            get
            {
                if (_payrollPeriodRepository == null)
                    _payrollPeriodRepository = new GenericRepository<PayrollPeriod>(_context);
                return _payrollPeriodRepository;
            }
        }

        public async Task SaveChangesAsync(string userId)
        {
            _context.CurrentUserId = userId;
            await _context.SaveChangesAsync();
        }
    }
}
