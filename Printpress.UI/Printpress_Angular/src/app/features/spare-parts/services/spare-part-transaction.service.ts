import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiPagingResponseDto } from '../../../core/models/api-response.dto';
import { SparePartTransactionDto } from '../models/spare-part-transaction.dto';

@Injectable({ providedIn: 'root' })
export class SparePartTransactionService {
  constructor(private httpService: HttpService) {}

  getByItemId(itemId: string, pageNumber: number, pageSize: number, dateFrom?: string, dateTo?: string, transactionType?: string): Observable<ApiPagingResponseDto<SparePartTransactionDto>> {
    const params: any = { pageNumber, pageSize };
    if (dateFrom) params.dateFrom = dateFrom;
    if (dateTo) params.dateTo = dateTo;
    if (transactionType) params.transactionType = transactionType;
    return this.httpService.get<ApiPagingResponseDto<SparePartTransactionDto>>(
      ApiUrlResource.SparePartTransactionAPI.getByItemId(itemId),
      params
    );
  }
}
