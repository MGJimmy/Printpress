import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import {
  InventoryConversionCreateDto,
  InventoryConversionListDto,
} from '../models/inventory-conversion.dto';

@Injectable({ providedIn: 'root' })
export class InventoryConversionService {
  constructor(private httpService: HttpService) {}

  create(dto: InventoryConversionCreateDto): Observable<unknown> {
    return this.httpService.post(ApiUrlResource.InventoryConversionAPI.create, dto);
  }

  getAll(
    categoryId?: number | null,
    itemId?: string | null,
    status?: string | null,
    isVoided?: boolean | null,
    dateFrom?: string,
    dateTo?: string,
  ): Observable<ApiResponseDto<InventoryConversionListDto>> {
    const params: Record<string, string | number | boolean> = {};
    if (categoryId != null) params['categoryId'] = categoryId;
    if (itemId) params['itemId'] = itemId;
    if (status) params['status'] = status;
    if (isVoided != null) params['isVoided'] = isVoided;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<InventoryConversionListDto>>(
      ApiUrlResource.InventoryConversionAPI.getAll,
      params,
    );
  }

  complete(id: string): Observable<ApiResponseDto<unknown>> {
    return this.httpService.post<ApiResponseDto<unknown>>(
      ApiUrlResource.InventoryConversionAPI.complete(id),
      {},
    );
  }

  void(id: string, reason?: string): Observable<ApiResponseDto<unknown>> {
    return this.httpService.post<ApiResponseDto<unknown>>(
      ApiUrlResource.InventoryConversionAPI.void(id),
      { reason: reason ?? '' },
    );
  }
}
