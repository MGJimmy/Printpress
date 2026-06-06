import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import {
  InventoryServicesUsageReportDto,
  ServiceCategoryFilterDto
} from '../models/inventory-services-usage-report.dto';
import { InventoryCategoryFilterDto } from '../models/order-inventory-items-report.dto';

@Injectable({ providedIn: 'root' })
export class InventoryServicesUsageReportService {
  constructor(private httpService: HttpService) {}

  getReport(
    inventoryItemCategoryId: number,
    serviceCategoryId: string,
    dateFrom?: string,
    dateTo?: string
  ): Observable<ApiResponseDto<InventoryServicesUsageReportDto>> {
    const params: any = { inventoryItemCategoryId, serviceCategoryId };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<InventoryServicesUsageReportDto>>(
      ApiUrlResource.ReportsAPI.inventoryServicesUsage, params
    );
  }

  getInventoryCategories(): Observable<ApiResponseDto<InventoryCategoryFilterDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryCategoryFilterDto[]>>(
      ApiUrlResource.InventoryAPI.CategoryBasicInfoAll
    );
  }

  getServiceCategories(): Observable<ApiResponseDto<ServiceCategoryFilterDto[]>> {
    return this.httpService.get<ApiResponseDto<ServiceCategoryFilterDto[]>>(
      ApiUrlResource.ReportsAPI.filterServiceCategories
    );
  }
}
