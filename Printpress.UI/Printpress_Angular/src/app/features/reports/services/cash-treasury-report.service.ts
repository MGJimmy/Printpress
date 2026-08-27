import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { CashTreasuryReportDto } from '../models/cash-treasury-report.dto';

@Injectable({ providedIn: 'root' })
export class CashTreasuryReportService {
  constructor(private httpService: HttpService) {}

  getReport(params: { dateFrom?: string; dateTo?: string }): Observable<ApiResponseDto<CashTreasuryReportDto>> {
    const query: Record<string, string> = {};
    if (params.dateFrom) query['dateFrom'] = params.dateFrom;
    if (params.dateTo) query['dateTo'] = params.dateTo;
    return this.httpService.get(ApiUrlResource.ReportsAPI.cashTreasury, query);
  }
}
