import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';
import { PayrollPeriodDto, PayrollPeriodCreateDto, PayrollPeriodDetailsDto } from '../models/payroll-period.dto';

@Injectable({ providedIn: 'root' })
export class PayrollPeriodService {
  constructor(private httpService: HttpService) {}

  getAll(pageSize: number, pageNumber: number): Observable<ApiPagingResponseDto<PayrollPeriodDto>> {

    let url = ApiUrlResource.PayrollPeriodAPI.getAll;
    const params: string[] = [];
    if (pageSize) params.push(`pageSize=${pageSize}`);
    if (pageNumber) params.push(`pageNumber=${pageNumber}`);
    if (params.length) url += '?' + params.join('&');
    return this.httpService.get<ApiPagingResponseDto<PayrollPeriodDto>>(url);
  }

  getOpenPeriods(): Observable<ApiResponseDto<PayrollPeriodDto[]>> {
    return this.httpService.get<ApiResponseDto<PayrollPeriodDto[]>>(ApiUrlResource.PayrollPeriodAPI.getOpenPeriods);
  }

  getById(id: string): Observable<ApiResponseDto<PayrollPeriodDetailsDto>> {
    return this.httpService.get<ApiResponseDto<PayrollPeriodDetailsDto>>(ApiUrlResource.PayrollPeriodAPI.getById(id));
  }

  add(payload: PayrollPeriodCreateDto): Observable<ApiResponseDto<PayrollPeriodDto>> {
    return this.httpService.post<ApiResponseDto<PayrollPeriodDto>>(ApiUrlResource.PayrollPeriodAPI.add, payload);
  }

  close(id: string): Observable<any> {
    return this.httpService.put<any>(ApiUrlResource.PayrollPeriodAPI.close(id), {});
  }
}
