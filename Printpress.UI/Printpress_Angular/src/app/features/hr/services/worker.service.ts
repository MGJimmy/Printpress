import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';
import { WorkerDto, WorkerCreateDto, WorkerUpdateDto, WorkerDetailsDto, WorkerProductionDto } from '../models/worker.dto';

@Injectable({ providedIn: 'root' })
export class WorkerService {
  constructor(private httpService: HttpService) {}

  getAll(pageSize?: number, pageNumber?: number): Observable<ApiPagingResponseDto<WorkerDto>> {
    let url = ApiUrlResource.WorkerAPI.getAll;
    const params: string[] = [];
    if (pageSize) params.push(`pageSize=${pageSize}`);
    if (pageNumber) params.push(`pageNumber=${pageNumber}`);
    if (params.length) url += '?' + params.join('&');
    return this.httpService.get<ApiPagingResponseDto<WorkerDto>>(url);
  }

  getActive(): Observable<ApiResponseDto<WorkerDto[]>> {
    return this.httpService.get<ApiResponseDto<WorkerDto[]>>(ApiUrlResource.WorkerAPI.getActive);
  }

  getById(id: string, productionDateFrom?: string, productionDateTo?: string): Observable<ApiResponseDto<WorkerDetailsDto>> {
    let url = ApiUrlResource.WorkerAPI.getById(id);
    const params: string[] = [];
    if (productionDateFrom) params.push(`productionDateFrom=${productionDateFrom}`);
    if (productionDateTo) params.push(`productionDateTo=${productionDateTo}`);
    if (params.length) url += '?' + params.join('&');
    return this.httpService.get<ApiResponseDto<WorkerDetailsDto>>(url);
  }

    getWorkerProduction(workerId: string, productionDateFrom?: string, productionDateTo?: string, 
      pageSize?: number, pageNumber?: number): Observable<ApiPagingResponseDto<WorkerProductionDto>> {
    let url = ApiUrlResource.WorkerAPI.getWorkerProduction(workerId);
    const params: string[] = [];
    if (productionDateFrom) params.push(`productionDateFrom=${productionDateFrom}`);
    if (productionDateTo) params.push(`productionDateTo=${productionDateTo}`);
    if (pageSize) params.push(`pageSize=${pageSize}`);
    if (pageNumber) params.push(`pageNumber=${pageNumber}`);
    if (params.length) url += '?' + params.join('&');
    return this.httpService.get<ApiPagingResponseDto<WorkerProductionDto>>(url);
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

  activate(id: string): Observable<any> {
    return this.httpService.put<any>(ApiUrlResource.WorkerAPI.activate(id), {});
  }
}
