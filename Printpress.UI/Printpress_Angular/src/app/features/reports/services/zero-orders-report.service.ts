import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { ZeroOrdersReportDto } from '../models/zero-orders-report.dto';

@Injectable({ providedIn: 'root' })
export class ZeroOrdersReportService {
  constructor(private httpService: HttpService) {}

  getReport(dateFrom?: string, dateTo?: string): Observable<ApiResponseDto<ZeroOrdersReportDto>> {
    const params: Record<string, string> = {};
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<ZeroOrdersReportDto>>(
      ApiUrlResource.ReportsAPI.zeroOrders,
      params,
    );
  }
}
