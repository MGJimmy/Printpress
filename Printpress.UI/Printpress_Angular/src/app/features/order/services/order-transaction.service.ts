import { Injectable } from '@angular/core';
import { HttpService } from '../../../core/services/http.service';
import { OrderTransactionGetDto } from '../models/order-transaction/order-transaction-get.dto';
import { OrderTransactionAddDto } from '../models/order-transaction/order-transaction-add.dto';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { Observable } from 'rxjs';
// import { ApiDataResponseDto } from '../../../core/models/api-data-response.dto';
import { PagingListDto } from '../../../core/models/paging-list.dto';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';

@Injectable({
  providedIn: 'root'
})
export class OrderTransactionService {

  constructor(private httpService:HttpService) { }

  addTransaction(OrderTransactionAddDto: OrderTransactionAddDto): Observable<ApiResponseDto<OrderTransactionGetDto>> {
    return this.httpService.post<ApiResponseDto<OrderTransactionGetDto>>(ApiUrlResource.OrderTransactionAPI.add, OrderTransactionAddDto);
  }

  getTransactions(orderId: number, pageNumber:number, pageSize:number): Observable<ApiPagingResponseDto<OrderTransactionGetDto>> {
    return this.httpService.get<ApiPagingResponseDto<OrderTransactionGetDto>>(ApiUrlResource.OrderTransactionAPI.getByPage, {orderId: orderId, pageNumber: pageNumber, pageSize: pageSize});
  }
}
