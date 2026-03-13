import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { InventoryTransactionDto } from '../models/inventory-transaction.dto';
import { ApiPagingResponseDto } from '../../../core/models/api-response.dto';

export interface StockOutDto {
  inventoryItemId: string;
  quantity: number;
  notes: string;
}

@Injectable({ providedIn: 'root' })
export class InventoryTransactionService {
  constructor(private httpService: HttpService) {}

  getByItemId(itemId: string, pageNumber: number, pageSize: number, dateFrom?: string, dateTo?: string, transactionType?: string): Observable<ApiPagingResponseDto<InventoryTransactionDto>> {
    const params: any = { pageNumber, pageSize };
    if (dateFrom) params.dateFrom = dateFrom;
    if (dateTo) params.dateTo = dateTo;
    if (transactionType) params.transactionType = transactionType;
    return this.httpService.get<ApiPagingResponseDto<InventoryTransactionDto>>(
      ApiUrlResource.InventoryTransactionAPI.getByItemId(itemId),
      params
    );
  }

  stockOut(dto: StockOutDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.InventoryTransactionAPI.stockOut, dto);
  }
}
