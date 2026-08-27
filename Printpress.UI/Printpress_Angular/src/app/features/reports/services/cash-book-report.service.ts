import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { CashBookReportDto } from '../models/cash-book-report.dto';

export interface CashBookReportParams {
  cashAccountId?: string;
  dateFrom?: string;
  dateTo?: string;
  type?: string;
  category?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class CashBookReportService {
  constructor(private httpService: HttpService) {}

  getReport(params: CashBookReportParams): Observable<ApiResponseDto<CashBookReportDto>> {
    const query: Record<string, string | number> = {};
    if (params.cashAccountId) query['cashAccountId'] = params.cashAccountId;
    if (params.dateFrom) query['dateFrom'] = params.dateFrom;
    if (params.dateTo) query['dateTo'] = params.dateTo;
    if (params.type) query['type'] = params.type;
    if (params.category) query['category'] = params.category;
    if (params.search) query['search'] = params.search;
    query['page'] = params.page ?? 1;
    query['pageSize'] = params.pageSize ?? 10;
    return this.httpService.get<ApiResponseDto<CashBookReportDto>>(
      ApiUrlResource.ReportsAPI.cashBook,
      query
    );
  }
}
