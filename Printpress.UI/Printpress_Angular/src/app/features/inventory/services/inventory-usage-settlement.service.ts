import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import {
  InventoryUsageSettlementCreateDto,
  InventoryUsageSettlementListDto,
} from '../models/inventory-usage-settlement.dto';

@Injectable({ providedIn: 'root' })
export class InventoryUsageSettlementService {
  constructor(private httpService: HttpService) {}

  create(dto: InventoryUsageSettlementCreateDto): Observable<unknown> {
    return this.httpService.post(ApiUrlResource.InventoryUsageSettlementAPI.create, dto);
  }

  getAll(
    categoryId?: number | null,
    itemId?: string | null,
    type?: string | null,
    dateFrom?: string,
    dateTo?: string,
  ): Observable<ApiResponseDto<InventoryUsageSettlementListDto>> {
    const params: Record<string, string | number> = {};
    if (categoryId != null) params['categoryId'] = categoryId;
    if (itemId) params['itemId'] = itemId;
    if (type) params['type'] = type;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<InventoryUsageSettlementListDto>>(
      ApiUrlResource.InventoryUsageSettlementAPI.getAll,
      params,
    );
  }

  void(id: string, reason?: string): Observable<ApiResponseDto<unknown>> {
    return this.httpService.post<ApiResponseDto<unknown>>(
      ApiUrlResource.InventoryUsageSettlementAPI.void(id),
      { reason: reason ?? '' },
    );
  }
}
