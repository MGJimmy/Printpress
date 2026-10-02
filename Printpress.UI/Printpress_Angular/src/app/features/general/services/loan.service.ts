import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';
import { LenderDto, LenderUpsertDto, LoanCreateDto, LoanDto, LoanListDto } from '../models/loan.dto';

@Injectable({ providedIn: 'root' })
export class LenderService {
  constructor(private http: HttpService) {}

  getAll(): Observable<ApiResponseDto<LenderDto[]>> {
    return this.http.get<ApiResponseDto<LenderDto[]>>(ApiUrlResource.LenderAPI.getAll);
  }

  getById(id: string): Observable<ApiResponseDto<LenderDto>> {
    return this.http.get<ApiResponseDto<LenderDto>>(ApiUrlResource.LenderAPI.getById(id));
  }

  add(payload: LenderUpsertDto): Observable<ApiResponseDto<LenderDto>> {
    return this.http.post<ApiResponseDto<LenderDto>>(ApiUrlResource.LenderAPI.add, payload);
  }

  update(id: string, payload: LenderUpsertDto): Observable<ApiResponseDto<LenderDto>> {
    return this.http.put<ApiResponseDto<LenderDto>>(ApiUrlResource.LenderAPI.update(id), payload);
  }

  delete(id: string): Observable<ApiResponseDto<unknown>> {
    return this.http.delete<ApiResponseDto<unknown>>(ApiUrlResource.LenderAPI.delete(id));
  }
}

@Injectable({ providedIn: 'root' })
export class LoanService {
  constructor(private http: HttpService) {}

  getAll(
    pageNumber: number,
    pageSize: number,
    filters?: {
      lenderId?: string;
      hasRemaining?: boolean | null;
      isVoided?: boolean | null;
      dateFrom?: string;
      dateTo?: string;
    }
  ): Observable<ApiPagingResponseDto<LoanListDto>> {
    const params: Record<string, string | number | boolean> = { pageNumber, pageSize };
    if (filters?.lenderId) params['lenderId'] = filters.lenderId;
    if (filters?.hasRemaining != null) params['hasRemaining'] = filters.hasRemaining;
    if (filters?.isVoided != null) params['isVoided'] = filters.isVoided;
    if (filters?.dateFrom) params['dateFrom'] = filters.dateFrom;
    if (filters?.dateTo) params['dateTo'] = filters.dateTo;
    return this.http.get<ApiPagingResponseDto<LoanListDto>>(ApiUrlResource.LoanAPI.getAll, params);
  }

  getById(id: string): Observable<ApiResponseDto<LoanDto>> {
    return this.http.get<ApiResponseDto<LoanDto>>(ApiUrlResource.LoanAPI.getById(id));
  }

  add(payload: LoanCreateDto): Observable<ApiResponseDto<LoanDto>> {
    return this.http.post<ApiResponseDto<LoanDto>>(ApiUrlResource.LoanAPI.add, payload);
  }

  pay(id: string, payload: { amount: number; occurredAt?: string; note?: string }): Observable<ApiResponseDto<unknown>> {
    return this.http.post<ApiResponseDto<unknown>>(ApiUrlResource.LoanAPI.pay(id), payload);
  }

  void(id: string, reason: string): Observable<ApiResponseDto<unknown>> {
    return this.http.post<ApiResponseDto<unknown>>(ApiUrlResource.LoanAPI.void(id), { reason });
  }

  voidPayment(id: string, paymentId: string, reason: string): Observable<ApiResponseDto<unknown>> {
    return this.http.post<ApiResponseDto<unknown>>(ApiUrlResource.LoanAPI.voidPayment(id), { paymentId, reason });
  }
}
