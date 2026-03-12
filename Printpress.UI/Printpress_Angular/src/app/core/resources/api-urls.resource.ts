export class ApiUrlResource {

  private static Account_URL = '/api/Account';
  public static readonly AccountAPI = {
    login: ApiUrlResource.Account_URL + '/login'
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
  };

  private static Inventory_URL = '/api/Inventory';
  public static readonly InventoryAPI = {
    getAll: ApiUrlResource.Inventory_URL + '/getAll',
    getByCategory: (categoryId: number) => `${ApiUrlResource.Inventory_URL}/getByCategory/${categoryId}`,
    getById: (id: string) => `${ApiUrlResource.Inventory_URL}/getById/${id}`,
    add: ApiUrlResource.Inventory_URL + '/add',
    update: (id: string) => `${ApiUrlResource.Inventory_URL}/update/${id}`,
    delete: (id: string) => `${ApiUrlResource.Inventory_URL}/delete/${id}`,
  };

  private static InventoryTransaction_URL = '/api/InventoryTransaction';
  public static readonly InventoryTransactionAPI = {
    getByItemId: (itemId: string) => `${ApiUrlResource.InventoryTransaction_URL}/getByItemId/${itemId}`,
    stockOut: ApiUrlResource.InventoryTransaction_URL + '/stock-out'
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
    getById: (id: string) => `${ApiUrlResource.SparePart_URL}/getById/${id}`,
    add: ApiUrlResource.SparePart_URL + '/add',
    update: (id: string) => `${ApiUrlResource.SparePart_URL}/update/${id}`,
    delete: (id: string) => `${ApiUrlResource.SparePart_URL}/delete/${id}`,
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
    getById: (id: string) => `${ApiUrlResource.PayrollPeriod_URL}/getById/${id}`,
    add: ApiUrlResource.PayrollPeriod_URL + '/add',
    close: (id: string) => `${ApiUrlResource.PayrollPeriod_URL}/close/${id}`
  };

  private static Reports_URL = '/api/Reports';
  public static readonly ReportsAPI = {
    orderInventoryItems: ApiUrlResource.Reports_URL + '/order-inventory-items',
    inventoryServicesUsage: ApiUrlResource.Reports_URL + '/inventory-services-usage',
    filterCategories: ApiUrlResource.Reports_URL + '/filter/inventory-categories',
    InventoryCategoryAll: ApiUrlResource.Reports_URL + '/filter/inventory-categories-All',
    filterItems: ApiUrlResource.Reports_URL + '/filter/inventory-items',
    filterServiceCategories: ApiUrlResource.Reports_URL + '/filter/service-categories'
  };

}
