export class ApiUrlResource {


  private static Cliet_URL = '/api/client';
  public static readonly ClientAPI = {
    getById: ApiUrlResource.Cliet_URL + '/getById',
    getByPage: ApiUrlResource.Cliet_URL + '/getByPage',
    getAll:ApiUrlResource.Cliet_URL + '/getAll',
    add: ApiUrlResource.Cliet_URL + '/add',
    update: (id:number) => `${ApiUrlResource.Cliet_URL}/update/${id}`,
    delete:(id:number) => `${ApiUrlResource.Cliet_URL}/delete/${id}`,
  };

  private static Order_URL = '/api/order';
  public static readonly OrderAPI = {
    getOrderById: ApiUrlResource.Order_URL + '/GetById',
    getordersSummaryList: ApiUrlResource.Order_URL + '/getOrderSummaryList',
    insertOrder: ApiUrlResource.Order_URL + '/insert',
  };

  private static Report_URL = '/report';
  public static readonly Report = {
    OrderReport: ApiUrlResource.Report_URL + '/orderReport',
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
    update: (id:number) => `${ApiUrlResource.Service_URL}/update/${id}`,
    delete:(id:number) => `${ApiUrlResource.Service_URL}/delete/${id}`,
  };

}
