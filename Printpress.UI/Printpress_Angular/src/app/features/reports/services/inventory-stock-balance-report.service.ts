import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { InventoryStockBalanceReportDto } from '../models/inventory-stock-balance-report.dto';
import { InventoryCategoryFilterDto } from './../models/order-inventory-items-report.dto';

@Injectable({ providedIn: 'root' })
export class InventoryStockBalanceReportService {
  constructor(private httpService: HttpService) {}

  getReport(
    categoryId?: number | null,
    dateFrom?: string,
    dateTo?: string,
  ): Observable<ApiResponseDto<InventoryStockBalanceReportDto>> {
    const params: Record<string, string | number> = {};
    if (categoryId != null) params['categoryId'] = categoryId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<InventoryStockBalanceReportDto>>(
      ApiUrlResource.ReportsAPI.inventoryStockBalance,
      params,
    );
  }

  getInventoryCategories(): Observable<ApiResponseDto<InventoryCategoryFilterDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryCategoryFilterDto[]>>(
      ApiUrlResource.InventoryAPI.CategoryBasicInfoAll,
    );
  }
}
