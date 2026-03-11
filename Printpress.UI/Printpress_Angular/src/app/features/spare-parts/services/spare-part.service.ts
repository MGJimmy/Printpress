import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { SparePartItemDto } from '../models/spare-part-item.dto';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';

export interface SparePartItemUpsertDto {
  name: string;
  packsPerCarton: number | null;
  unitsPerPack: number | null;
}

@Injectable({ providedIn: 'root' })
export class SparePartService {
  constructor(private httpService: HttpService) {}

  getAll(pageSize: number, pageNumber: number): Observable<ApiPagingResponseDto<SparePartItemDto>> {
    return this.httpService.get<ApiPagingResponseDto<SparePartItemDto>>(
      ApiUrlResource.SparePartAPI.getAll,
      { pageSize, pageNumber }
    );
  }

  getById(id: string): Observable<ApiResponseDto<SparePartItemDto>> {
    return this.httpService.get<ApiResponseDto<SparePartItemDto>>(ApiUrlResource.SparePartAPI.getById(id));
  }

  add(payload: SparePartItemUpsertDto): Observable<ApiResponseDto<SparePartItemDto>> {
    return this.httpService.post<ApiResponseDto<SparePartItemDto>>(ApiUrlResource.SparePartAPI.add, payload);
  }

  update(id: string, payload: SparePartItemUpsertDto): Observable<ApiResponseDto<SparePartItemDto>> {
    return this.httpService.put<ApiResponseDto<SparePartItemDto>>(ApiUrlResource.SparePartAPI.update(id), payload);
  }

  delete(id: string): Observable<any> {
    return this.httpService.delete<any>(ApiUrlResource.SparePartAPI.delete(id));
  }
}
