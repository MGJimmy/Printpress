import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { InventoryItemDto } from '../models/inventory-item.dto';
import { InventoryItemSelectionDto } from '../models/inventory-item-selection.dto';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';

export interface InventoryItemUpsertDto {
  name: string;
  inventoryItemCategory: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
  expectedPurchaseLossPercent: number;
  expectedProductionWastePercent: number;
}

@Injectable({ providedIn: 'root' })
export class InventoryService {
  constructor(private httpService: HttpService) {}

  getAll(pageSize: number, pageNumber: number): Observable<ApiPagingResponseDto<InventoryItemDto>> {
    return this.httpService.get<ApiPagingResponseDto<InventoryItemDto>>(
      ApiUrlResource.InventoryAPI.getAll,
      { pageSize, pageNumber }
    );
  }

  getAllForSelection(): Observable<ApiResponseDto<InventoryItemSelectionDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryItemSelectionDto[]>>(
      ApiUrlResource.InventoryAPI.getAllForSelection
    );
  }

  getByCategory(categoryId: number): Observable<ApiResponseDto<InventoryItemDto[]>> {
    return this.httpService.get<ApiResponseDto<InventoryItemDto[]>>(ApiUrlResource.InventoryAPI.getByCategory(categoryId));
  }

  getById(id: string): Observable<ApiResponseDto<InventoryItemDto>> {
    return this.httpService.get<ApiResponseDto<InventoryItemDto>>(ApiUrlResource.InventoryAPI.getById(id));
  }

  add(payload: InventoryItemUpsertDto): Observable<ApiResponseDto<InventoryItemDto>> {
    return this.httpService.post<ApiResponseDto<InventoryItemDto>>(ApiUrlResource.InventoryAPI.add, payload);
  }

  update(id: string, payload: InventoryItemUpsertDto): Observable<ApiResponseDto<InventoryItemDto>> {
    return this.httpService.put<ApiResponseDto<InventoryItemDto>>(ApiUrlResource.InventoryAPI.update(id), payload);
  }

  delete(id: string): Observable<any> {
    return this.httpService.delete<any>(ApiUrlResource.InventoryAPI.delete(id));
  }

  deactivate(id: string): Observable<any> {
    return this.httpService.put<any>(ApiUrlResource.InventoryAPI.deactivate(id), {});
  }

  activate(id: string): Observable<any> {
    return this.httpService.put<any>(ApiUrlResource.InventoryAPI.activate(id), {});
  }
}
