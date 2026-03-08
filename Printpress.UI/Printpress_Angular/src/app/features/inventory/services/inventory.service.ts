import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { InventoryItemDto } from '../models/inventory-item.dto';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  constructor(private httpService: HttpService) {}

  getAll(pageSize: number, pageNumber: number): Observable<ApiPagingResponseDto<InventoryItemDto>> {
    return this.httpService.get<ApiPagingResponseDto<InventoryItemDto>>(
      ApiUrlResource.InventoryAPI.getAll,
      { pageSize, pageNumber }
    );
  }

  getById(id: string): Observable<ApiResponseDto<InventoryItemDto>> {
    return this.httpService.get<ApiResponseDto<InventoryItemDto>>(ApiUrlResource.InventoryAPI.getById(id));
  }

  delete(id: string): Observable<any> {
    return this.httpService.delete<any>(ApiUrlResource.InventoryAPI.delete(id));
  }
}
