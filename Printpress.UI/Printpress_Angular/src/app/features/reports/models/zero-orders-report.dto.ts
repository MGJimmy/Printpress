export interface ZeroOrderReportRowDto {
  orderId: string;
  orderName: string;
  clientName: string;
  createdAt: string;
  status: string;
  serviceCount: number;
  itemCount: number;
  totalPrice: number;
}

export interface ZeroOrdersReportDto {
  orders: ZeroOrderReportRowDto[];
  orderCount: number;
  itemCount: number;
}
