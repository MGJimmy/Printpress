export class ApiUrlResource {

  private static Account_URL = '/api/Account';
  public static readonly AccountAPI = {
    login: ApiUrlResource.Account_URL + '/login',
    refreshToken: ApiUrlResource.Account_URL + '/refreshToken',
    getAllUsers: ApiUrlResource.Account_URL + '/get-all-users',
    getAllRoles: ApiUrlResource.Account_URL + '/get-all-roles',
    createUser: ApiUrlResource.Account_URL + '/create-user',
    updateUser: ApiUrlResource.Account_URL + '/update-user',
    deleteUser: (id: string) => `${ApiUrlResource.Account_URL}/delete-user/${id}`,
    changePassword: ApiUrlResource.Account_URL + '/change-password',
    updatePassword: ApiUrlResource.Account_URL + '/update-password',
  };

  private static Cliet_URL = '/api/client';
  public static readonly ClientAPI = {
    getById: ApiUrlResource.Cliet_URL + '/getById',
    getByPage: ApiUrlResource.Cliet_URL + '/getByPage',
    getAll:ApiUrlResource.Cliet_URL + '/getAll',
    add: ApiUrlResource.Cliet_URL + '/add',
    update: (id:string) => `${ApiUrlResource.Cliet_URL}/update/${id}`,
    delete:(id:string) => `${ApiUrlResource.Cliet_URL}/delete/${id}`,
  };

  private static ItemServiceExecution_URL = '/api/ItemServiceExecution';
  public static readonly ItemServiceExecutionAPI = {
    groupItems: (groupId: string) => `${ApiUrlResource.ItemServiceExecution_URL}/group-items/${groupId}`,
    itemSummary: (itemId: string) => `${ApiUrlResource.ItemServiceExecution_URL}/item-summary/${itemId}`,
    itemHistory: (itemId: string) => `${ApiUrlResource.ItemServiceExecution_URL}/item-history/${itemId}`,
    execute: ApiUrlResource.ItemServiceExecution_URL + '/execute',
    completeItem: ApiUrlResource.ItemServiceExecution_URL + '/complete'
  };

  private static Order_URL = '/api/order';
  public static readonly OrderAPI = {
    getOrderById: ApiUrlResource.Order_URL + '/GetById',
    getOrderMainData: ApiUrlResource.Order_URL + '/GetMainData',
    getordersSummaryList: ApiUrlResource.Order_URL + '/getOrderSummaryList',
    insertOrder: ApiUrlResource.Order_URL + '/insert',
    updateOrder: ApiUrlResource.Order_URL + '/update',
    deliverOrderGroup: ApiUrlResource.Order_URL + '/deliverOrderGroup',
    delete: (id:string) => `${ApiUrlResource.Order_URL}/delete/${id}`,
  };

  private static Report_URL = '/api/report';
  public static readonly Report = {
    OrderReport:  ApiUrlResource.Report_URL + `/generateReport`, 
  };

  private static OrderTransaction_URL = '/api/OrderTransaction';
  public static readonly OrderTransactionAPI = {
    getByPage: ApiUrlResource.OrderTransaction_URL + '/getByPage',
    add: ApiUrlResource.OrderTransaction_URL + '/add'
  };

  private static ServiceCategory_URL = '/api/ServiceCategory';
  public static readonly ServiceCategoryAPI = {
    getAll: ApiUrlResource.ServiceCategory_URL + '/getAll'
  };

  private static Service_URL = '/api/service';
  public static readonly ServiceAPI = {
    getAll: ApiUrlResource.Service_URL + '/getAll',
    add: ApiUrlResource.Service_URL + '/add',
    update: (id:string) => `${ApiUrlResource.Service_URL}/update/${id}`,
    delete:(id:string) => `${ApiUrlResource.Service_URL}/delete/${id}`,
    deactivate: (id: string) => `${ApiUrlResource.Service_URL}/deactivate/${id}`,
  };

  private static Inventory_URL = '/api/Inventory';
  public static readonly InventoryAPI = {
    getAll: ApiUrlResource.Inventory_URL + '/getAll',
    getAllForSelection: ApiUrlResource.Inventory_URL + '/getAllForSelection',
    getByCategory: (categoryId: number) => `${ApiUrlResource.Inventory_URL}/getByCategory/${categoryId}`,
    getById: (id: string) => `${ApiUrlResource.Inventory_URL}/getById/${id}`,
    add: ApiUrlResource.Inventory_URL + '/add',
    update: (id: string) => `${ApiUrlResource.Inventory_URL}/update/${id}`,
    delete: (id: string) => `${ApiUrlResource.Inventory_URL}/delete/${id}`,
    deactivate: (id: string) => `${ApiUrlResource.Inventory_URL}/deactivate/${id}`,
    activate: (id: string) => `${ApiUrlResource.Inventory_URL}/activate/${id}`,

    InventoryCategoryBasicInfo: ApiUrlResource.Inventory_URL + '/inventory-categories',
    CategoryBasicInfoAll: ApiUrlResource.Inventory_URL + '/inventory-categories-All',
    ItemBasicInfo: ApiUrlResource.Inventory_URL + '/inventory-items',

  };

  private static InventoryTransaction_URL = '/api/InventoryTransaction';
  public static readonly InventoryTransactionAPI = {
    getByItemId: (itemId: string) => `${ApiUrlResource.InventoryTransaction_URL}/getByItemId/${itemId}`,
    stockOut: ApiUrlResource.InventoryTransaction_URL + '/stock-out',
    getByWorkerId: ApiUrlResource.InventoryTransaction_URL + '/getByWorkerId'
  };

  private static FileUpload_URL = '/api/FileUpload';
  public static readonly FileUploadAPI = {
    upload: ApiUrlResource.FileUpload_URL + '/upload'
  };

  private static PurchaseInvoice_URL = '/api/PurchaseInvoice';
  public static readonly PurchaseInvoiceAPI = {
    add: ApiUrlResource.PurchaseInvoice_URL + '/add'
  };

  private static SparePart_URL = '/api/SparePart';
  public static readonly SparePartAPI = {
    getAll: ApiUrlResource.SparePart_URL + '/getAll',
    getAllForSelection: ApiUrlResource.SparePart_URL + '/getAllForSelection',
    getById: (id: string) => `${ApiUrlResource.SparePart_URL}/getById/${id}`,
    add: ApiUrlResource.SparePart_URL + '/add',
    update: (id: string) => `${ApiUrlResource.SparePart_URL}/update/${id}`,
    delete: (id: string) => `${ApiUrlResource.SparePart_URL}/delete/${id}`,
  };

  private static SparePartTransaction_URL = '/api/SparePartTransaction';
  public static readonly SparePartTransactionAPI = {
    getByItemId: (itemId: string) => `${ApiUrlResource.SparePartTransaction_URL}/getByItemId/${itemId}`,
  };

  private static SparePartPurchaseInvoice_URL = '/api/SparePartPurchaseInvoice';
  public static readonly SparePartPurchaseInvoiceAPI = {
    add: ApiUrlResource.SparePartPurchaseInvoice_URL + '/add'
  };

  private static SparePartSellingInvoice_URL = '/api/SparePartSellingInvoice';
  public static readonly SparePartSellingInvoiceAPI = {
    add: ApiUrlResource.SparePartSellingInvoice_URL + '/add'
  };

  private static PayrollPeriod_URL = '/api/PayrollPeriod';
  public static readonly PayrollPeriodAPI = {
    getAll: ApiUrlResource.PayrollPeriod_URL + '/getAll',
    getOpenPeriods: ApiUrlResource.PayrollPeriod_URL + '/getOpenPeriods',
    getById: (id: string) => `${ApiUrlResource.PayrollPeriod_URL}/getById/${id}`,
    add: ApiUrlResource.PayrollPeriod_URL + '/add',
    close: (id: string) => `${ApiUrlResource.PayrollPeriod_URL}/close/${id}`
  };

  private static Worker_URL = '/api/Worker';
  public static readonly WorkerAPI = {
    getAll: ApiUrlResource.Worker_URL + '/getAll',
    getActive: ApiUrlResource.Worker_URL + '/getActive',
    getById: (id: string) => `${ApiUrlResource.Worker_URL}/getById/${id}`,
    getWorkerProduction: (workerId: string) => `${ApiUrlResource.Worker_URL}/getWorkerProduction/${workerId}`,
    add: ApiUrlResource.Worker_URL + '/add',
    update: ApiUrlResource.Worker_URL + '/update',
    deactivate: (id: string) => `${ApiUrlResource.Worker_URL}/deactivate/${id}`,
    activate: (id: string) => `${ApiUrlResource.Worker_URL}/activate/${id}`
  };

  private static WorkerSalaryTransaction_URL = '/api/WorkerSalaryTransaction';
  public static readonly WorkerSalaryTransactionAPI = {
    add: ApiUrlResource.WorkerSalaryTransaction_URL + '/add',
    delete: (id: string) => `${ApiUrlResource.WorkerSalaryTransaction_URL}/delete/${id}`
  };

  private static CashAccount_URL = '/api/CashAccount';
  public static readonly CashAccountAPI = {
    getAll: ApiUrlResource.CashAccount_URL + '/getAll',
    getById: (id: string) => `${ApiUrlResource.CashAccount_URL}/getById/${id}`,
  };

  private static CashTransaction_URL = '/api/CashTransaction';
  public static readonly CashTransactionAPI = {
    getByCashAccountId: (cashAccountId: string) => `${ApiUrlResource.CashTransaction_URL}/getByCashAccountId/${cashAccountId}`,
    add: ApiUrlResource.CashTransaction_URL + '/add',
    void: (id: string) => `${ApiUrlResource.CashTransaction_URL}/void/${id}`,
    transfer: ApiUrlResource.CashTransaction_URL + '/transfer',
    externalOrders: ApiUrlResource.CashTransaction_URL + '/external-orders',
  };

  private static Reports_URL = '/api/Reports';
  public static readonly ReportsAPI = {
    orderInventoryItems: ApiUrlResource.Reports_URL + '/order-inventory-items',
    inventoryServicesUsage: ApiUrlResource.Reports_URL + '/inventory-services-usage',
    filterServiceCategories: ApiUrlResource.Reports_URL + '/filter/service-categories',
    cashBook: ApiUrlResource.Reports_URL + '/cash-book',
    cashReconcile: ApiUrlResource.Reports_URL + '/cash-reconcile',
  };

}
