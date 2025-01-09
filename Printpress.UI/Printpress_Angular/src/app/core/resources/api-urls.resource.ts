export class ApiUrlResource {


  private static Cliet_URL = '/api/client';
  public static readonly ClientAPI = {
    getById: ApiUrlResource.Cliet_URL + '/getById',
    getByPage: ApiUrlResource.Cliet_URL + '/getByPage',
    add: ApiUrlResource.Cliet_URL + '/add',
    update: (id:number) => `${ApiUrlResource.Cliet_URL}/update/${id}`,
    delete:(id:number) => `${ApiUrlResource.Cliet_URL}/delete/${id}`,
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
