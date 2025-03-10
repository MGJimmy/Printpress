export  class OrderSummaryDto {
    constructor(
      public orderName: string,
      public clientName: string,
      public totalAmount: number,
      public paidAmount: number,
      public createdAt: string,
      public orderStatus: number,
      public id: number
    ) {}
  }