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
    add: ApiUrlResource.Inventory_URL + '/add',
    update: (id: string) => `${ApiUrlResource.Inventory_URL}/update/${id}`,
    delete: (id: string) => `${ApiUrlResource.Inventory_URL}/delete/${id}`,
  };

}
