import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { InventoryMovementReportDto } from '../models/inventory-movement-report.dto';
import { InventoryCategoryFilterDto, InventoryItemFilterDto } from '../models/order-inventory-items-report.dto';

@Injectable({ providedIn: 'root' })
export class InventoryMovementReportService {
  constructor(private httpService: HttpService) {}

  getReport(
    inventoryItemId: string,
    dateFrom?: string,
    dateTo?: string,
  ): Observable<ApiResponseDto<InventoryMovementReportDto>> {
    const params: Record<string, string> = { inventoryItemId };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<InventoryMovementReportDto>>(
      ApiUrlResource.ReportsAPI.inventoryMovement,
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
