import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { OrderSummaryDto } from '../models/order/order-summary.Dto';
import { ApiPagingResponseDto } from '../../../core/models/api-response.dto';
import { OrderUpsertDto } from '../models/order/order-upsert.Dto';
@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(private httpService: HttpService) {}

  public getOrderById(id: number): Observable<any> {
    return this.httpService.get<any>(
      ApiUrlResource.OrderAPI.getOrderById + `/${id}`
    );
  }

  public getOrderMainData(id: number): Observable<any> {
    return this.httpService.get<any>(
      ApiUrlResource.OrderAPI.getOrderMainData + `/${id}`
    );
  }

  public getOrdersSummaryList(
    pageSize: number,
    pageNumber: number
  ): Observable<ApiPagingResponseDto<OrderSummaryDto>> {
    return this.httpService.get<ApiPagingResponseDto<OrderSummaryDto>>(
      ApiUrlResource.OrderAPI.getordersSummaryList,
      { pageSize: pageSize, pageNumber: pageNumber }
    );
  }

  public insertOrder(orderDTO: OrderUpsertDto): Observable<any> {
    console.log(JSON.stringify( orderDTO));
    return this.httpService.post<OrderUpsertDto>(
      ApiUrlResource.OrderAPI.insertOrder,
      orderDTO
    );
  }

  public updateOrder(orderDTO: OrderUpsertDto): Observable<any> {
    return this.httpService.put<OrderUpsertDto>(
      `${ApiUrlResource.OrderAPI.updateOrder}/${orderDTO.id}`,
      orderDTO
    );
  }
  

  // deleteOrder(id: number): Observable<any> {
  //     return this.httpService.delete<any>(`${this.apiUrl}/${id}`);
  // }
}
