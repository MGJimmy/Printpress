import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { CashReconcileReportDto } from '../models/cash-reconcile-report.dto';

export interface CashReconcileReportParams {
  cashAccountId?: string;
  dateFrom?: string;
  dateTo?: string;
}

@Injectable({ providedIn: 'root' })
export class CashReconcileReportService {
  constructor(private httpService: HttpService) {}

  getReport(params: CashReconcileReportParams): Observable<ApiResponseDto<CashReconcileReportDto>> {
    const query: Record<string, string> = {};
    if (params.cashAccountId) query['cashAccountId'] = params.cashAccountId;
    if (params.dateFrom) query['dateFrom'] = params.dateFrom;
    if (params.dateTo) query['dateTo'] = params.dateTo;
    return this.httpService.get<ApiResponseDto<CashReconcileReportDto>>(
      ApiUrlResource.ReportsAPI.cashReconcile,
      query
    );
  }
}
