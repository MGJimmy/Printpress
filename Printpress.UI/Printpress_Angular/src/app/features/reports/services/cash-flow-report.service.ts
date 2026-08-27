import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { CashFlowReportDto } from '../models/cash-flow-report.dto';

@Injectable({ providedIn: 'root' })
export class CashFlowReportService {
  constructor(private httpService: HttpService) {}

  getReport(params: { cashAccountId?: string; dateFrom?: string; dateTo?: string }): Observable<ApiResponseDto<CashFlowReportDto>> {
    const query: Record<string, string> = {};
    if (params.cashAccountId) query['cashAccountId'] = params.cashAccountId;
    if (params.dateFrom) query['dateFrom'] = params.dateFrom;
    if (params.dateTo) query['dateTo'] = params.dateTo;
    return this.httpService.get(ApiUrlResource.ReportsAPI.cashFlow, query);
  }
}
