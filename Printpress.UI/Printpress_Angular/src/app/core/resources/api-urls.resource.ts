export class ApiUrlResource {


  private static Cliet_URL = '/api/client';
  public static readonly ClientAPI = {
    getClientById: ApiUrlResource.Cliet_URL + '/getClientById',
  };

  private static Order_URL = '/api/order';
  public static readonly OrderAPI = {
    getOrderById: ApiUrlResource.Order_URL + '/getOrderById',
  };

  private static Report_URL = '/report';
  public static readonly Report = {
    OrderReport: ApiUrlResource.Report_URL + '/orderReport',
  };

  
}
