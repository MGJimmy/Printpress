import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { CashTransactionDto, ExternalOrderDto } from '../models/cash-transaction.dto';
import { AddCashTransactionDto } from '../models/add-cash-transaction.dto';
import { TransferCashTransactionDto } from '../models/transfer-cash-transaction.dto';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';

@Injectable({ providedIn: 'root' })
export class CashTransactionService {
  constructor(private httpService: HttpService) {}

  getByCashAccountId(
    cashAccountId: string,
    pageNumber: number,
    pageSize: number,
    dateFrom?: string,
    dateTo?: string,
    type?: string,
    category?: string
  ): Observable<ApiPagingResponseDto<CashTransactionDto>> {
    const params: any = { pageNumber, pageSize };
    if (dateFrom) params.dateFrom = dateFrom;
    if (dateTo) params.dateTo = dateTo;
    if (type) params.type = type;
    if (category) params.category = category;
    return this.httpService.get<ApiPagingResponseDto<CashTransactionDto>>(
      ApiUrlResource.CashTransactionAPI.getByCashAccountId(cashAccountId),
      params
    );
  }

  add(payload: AddCashTransactionDto): Observable<ApiResponseDto<CashTransactionDto>> {
    return this.httpService.post<ApiResponseDto<CashTransactionDto>>(ApiUrlResource.CashTransactionAPI.add, payload);
  }

  void(id: string, reason?: string): Observable<ApiResponseDto<void>> {
    return this.httpService.post<ApiResponseDto<void>>(ApiUrlResource.CashTransactionAPI.void(id), { reason: reason ?? '' });
  }

  transfer(payload: TransferCashTransactionDto): Observable<ApiResponseDto<void>> {
    return this.httpService.post<ApiResponseDto<void>>(ApiUrlResource.CashTransactionAPI.transfer, payload);
  }

  getExternalOrders(): Observable<ApiResponseDto<ExternalOrderDto[]>> {
    return this.httpService.get<ApiResponseDto<ExternalOrderDto[]>>(ApiUrlResource.CashTransactionAPI.externalOrders);
  }
}
