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

  public getOrderById(id: string): Observable<any> {
    return this.httpService.get<any>(
      ApiUrlResource.OrderAPI.getOrderById + `/${id}`
    );
  }

  public getOrderMainData(id: string): Observable<any> {
    return this.httpService.get<any>(
      ApiUrlResource.OrderAPI.getOrderMainData + `/${id}`
    );
  }

  public getOrdersSummaryList(
    pageSize: number,
    pageNumber: number,
    filters?: {
      search?: string;
      clientId?: string;
      status?: number;
      isZeroOrder?: boolean;
      dateFrom?: string;
      dateTo?: string;
    }
  ): Observable<ApiPagingResponseDto<OrderSummaryDto>> {
    const params: { [param: string]: string | number | boolean } = {
      pageSize,
      pageNumber
    };
    if (filters?.search) params['search'] = filters.search;
    if (filters?.clientId) params['clientId'] = filters.clientId;
    if (filters?.status != null) params['status'] = filters.status;
    if (filters?.isZeroOrder != null) params['isZeroOrder'] = filters.isZeroOrder;
    if (filters?.dateFrom) params['dateFrom'] = filters.dateFrom;
    if (filters?.dateTo) params['dateTo'] = filters.dateTo;

    return this.httpService.get<ApiPagingResponseDto<OrderSummaryDto>>(
      ApiUrlResource.OrderAPI.getordersSummaryList,
      params
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
  
  public deleteOrder(id: string): Observable<any> {
    return this.httpService.delete<any>(ApiUrlResource.OrderAPI.delete(id));
  }

  public deliverOrderGroup(deliverGroupDto:any): Observable<any> {
    return this.httpService.post<any>( `${ApiUrlResource.OrderAPI.deliverOrderGroup}`,deliverGroupDto);
  }
}
