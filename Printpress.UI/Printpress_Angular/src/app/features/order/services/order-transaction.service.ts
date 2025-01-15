import { Injectable } from '@angular/core';
import { HttpService } from '../../../core/services/http.service';
import { OrderTransactionGetDto } from '../models/order-transaction/order-transaction-get.dto';
import { OrderTransactionAddDto } from '../models/order-transaction/order-transaction-add.dto';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { Observable } from 'rxjs';
import { ApiDataResponseDto } from '../../../core/models/api-data-response.dto';
import { PagingListDto } from '../../../core/models/paging-list.dto';

@Injectable({
  providedIn: 'root'
})
export class OrderTransactionService {

  constructor(private httpService:HttpService) { }

  addTransaction(OrderTransactionAddDto: OrderTransactionAddDto): Observable<ApiDataResponseDto<OrderTransactionGetDto>> {
    return this.httpService.post<ApiDataResponseDto<OrderTransactionGetDto>>(ApiUrlResource.OrderTransactionAPI.add, OrderTransactionAddDto);
  }

  getTransactions(orderId: number, pageNumber:number, pageSize:number): Observable<ApiDataResponseDto<PagingListDto<OrderTransactionGetDto>>> {
    return this.httpService.get<ApiDataResponseDto<PagingListDto<OrderTransactionGetDto>>>(ApiUrlResource.OrderTransactionAPI.getByPage, {orderId: orderId, pageNumber:pageNumber, pageSize:pageSize});
  }
}
