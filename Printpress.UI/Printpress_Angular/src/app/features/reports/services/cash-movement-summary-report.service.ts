import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { CashMovementSummaryReportDto } from '../models/cash-movement-summary-report.dto';

export interface CashMovementSummaryParams {
  cashAccountId?: string;
  dateFrom?: string;
  dateTo?: string;
}

@Injectable({ providedIn: 'root' })
export class CashMovementSummaryReportService {
  constructor(private httpService: HttpService) {}

  getReport(params: CashMovementSummaryParams): Observable<ApiResponseDto<CashMovementSummaryReportDto>> {
    const query: Record<string, string> = {};
    if (params.cashAccountId) query['cashAccountId'] = params.cashAccountId;
    if (params.dateFrom) query['dateFrom'] = params.dateFrom;
    if (params.dateTo) query['dateTo'] = params.dateTo;
    return this.httpService.get<ApiResponseDto<CashMovementSummaryReportDto>>(
      ApiUrlResource.ReportsAPI.cashMovementSummary,
      query
    );
  }
}
