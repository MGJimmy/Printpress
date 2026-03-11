import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { OrderInventoryItemsReportDto, InventoryCategoryFilterDto, InventoryItemFilterDto } from '../models/order-inventory-items-report.dto';

@Injectable({ providedIn: 'root' })
export class OrderInventoryItemsReportService {
  constructor(private httpService: HttpService) {}

  getReport(inventoryItemId: string, dateFrom?: string, dateTo?: string): Observable<ApiResponseDto<OrderInventoryItemsReportDto>> {
    const params: any = { inventoryItemId };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<OrderInventoryItemsReportDto>>(ApiUrlResource.ReportsAPI.orderInventoryItems, params);
  }

  getCategories(): Observable<ApiResponseDto<InventoryCategoryFilterDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryCategoryFilterDto[]>>(ApiUrlResource.ReportsAPI.filterCategories);
  }

  getItemsByCategory(categoryId: number): Observable<ApiResponseDto<InventoryItemFilterDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryItemFilterDto[]>>(ApiUrlResource.ReportsAPI.filterItems, { categoryId });
  }
}
