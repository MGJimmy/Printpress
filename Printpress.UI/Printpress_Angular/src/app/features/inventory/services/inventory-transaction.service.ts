import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { InventoryTransactionDto } from '../models/inventory-transaction.dto';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';
import { InventoryTransactionListDto } from '../models/inventory-document-list.dto';

export interface StockOutDto {
  inventoryItemId: string;
  quantity: number;
  notes: string;
  workerId?: string;
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

  getAll(
    categoryId?: number | null,
    itemId?: string | null,
    workerId?: string | null,
    type?: string | null,
    dateFrom?: string,
    dateTo?: string,
  ): Observable<ApiResponseDto<InventoryTransactionListDto>> {
    const params: Record<string, string | number> = {};
    if (categoryId != null) params['categoryId'] = categoryId;
    if (itemId) params['itemId'] = itemId;
    if (workerId) params['workerId'] = workerId;
    if (type) params['type'] = type;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<InventoryTransactionListDto>>(
      ApiUrlResource.InventoryTransactionAPI.getAll,
      params,
    );
  }

  getByWorkerId(
    workerId: string,
    pageNumber: number,
    pageSize: number,
    inventoryItemCategoryId?: number,
    inventoryItemId?: string,
    dateFrom?: string,
    dateTo?: string
  ): Observable<InventoryTransactionDto[]> {
    const params: any = { pageNumber, pageSize };
    if (inventoryItemCategoryId !== undefined && inventoryItemCategoryId !== null) params['inventoryItemCategoryId'] = inventoryItemCategoryId;
    if (inventoryItemId) params['inventoryItemId'] = inventoryItemId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<InventoryTransactionDto[]>(`${ApiUrlResource.InventoryTransactionAPI.getByWorkerId}/${workerId}`, params);
  }
}
