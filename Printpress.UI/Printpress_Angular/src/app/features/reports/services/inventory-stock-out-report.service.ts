import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { InventoryStockOutReportDto } from '../models/inventory-stock-out-report.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../models/order-inventory-items-report.dto';

@Injectable({ providedIn: 'root' })
export class InventoryStockOutReportService {
  constructor(private httpService: HttpService) {}

  getReport(
    categoryId?: number | null,
    inventoryItemId?: string | null,
    workerId?: string | null,
    dateFrom?: string,
    dateTo?: string,
  ): Observable<ApiResponseDto<InventoryStockOutReportDto>> {
    const params: Record<string, string | number> = {};
    if (categoryId != null) params['categoryId'] = categoryId;
    if (inventoryItemId) params['inventoryItemId'] = inventoryItemId;
    if (workerId) params['workerId'] = workerId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<InventoryStockOutReportDto>>(
      ApiUrlResource.ReportsAPI.inventoryStockOut,
      params,
    );
  }

  getInventoryCategories(): Observable<ApiResponseDto<InventoryCategoryFilterDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryCategoryFilterDto[]>>(
      ApiUrlResource.InventoryAPI.CategoryBasicInfoAll,
    );
  }

  getItemsByCategory(categoryId: number): Observable<ApiResponseDto<InventoryItemFilterDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryItemFilterDto[]>>(
      ApiUrlResource.InventoryAPI.getByCategory(categoryId),
    );
  }
}
