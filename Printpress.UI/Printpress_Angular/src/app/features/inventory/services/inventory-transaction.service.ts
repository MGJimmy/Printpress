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

  getByItemId(itemId: string, pageNumber: number, pageSize: number): Observable<ApiPagingResponseDto<InventoryTransactionDto>> {
    return this.httpService.get<ApiPagingResponseDto<InventoryTransactionDto>>(
      ApiUrlResource.InventoryTransactionAPI.getByItemId(itemId),
      { pageNumber, pageSize }
    );
  }

  stockOut(dto: StockOutDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.InventoryTransactionAPI.stockOut, dto);
  }
}
