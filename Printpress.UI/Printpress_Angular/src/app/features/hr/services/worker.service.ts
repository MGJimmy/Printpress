import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { WorkerDto, WorkerCreateDto, WorkerUpdateDto, WorkerDetailsDto } from '../models/worker.dto';

@Injectable({ providedIn: 'root' })
export class WorkerService {
  constructor(private httpService: HttpService) {}

  getAll(): Observable<ApiResponseDto<WorkerDto[]>> {
    return this.httpService.get<ApiResponseDto<WorkerDto[]>>(ApiUrlResource.WorkerAPI.getAll);
  }

  getById(id: string, productionDateFrom?: string, productionDateTo?: string): Observable<ApiResponseDto<WorkerDetailsDto>> {
    let url = ApiUrlResource.WorkerAPI.getById(id);
    const params: string[] = [];
    if (productionDateFrom) params.push(`productionDateFrom=${productionDateFrom}`);
    if (productionDateTo) params.push(`productionDateTo=${productionDateTo}`);
    if (params.length) url += '?' + params.join('&');
    return this.httpService.get<ApiResponseDto<WorkerDetailsDto>>(url);
  }

  add(payload: WorkerCreateDto): Observable<ApiResponseDto<WorkerDto>> {
    return this.httpService.post<ApiResponseDto<WorkerDto>>(ApiUrlResource.WorkerAPI.add, payload);
  }

  update(payload: WorkerUpdateDto): Observable<ApiResponseDto<WorkerDto>> {
    return this.httpService.put<ApiResponseDto<WorkerDto>>(ApiUrlResource.WorkerAPI.update, payload);
  }

  deactivate(id: string): Observable<any> {
    return this.httpService.put<any>(ApiUrlResource.WorkerAPI.deactivate(id), {});
  }
}
