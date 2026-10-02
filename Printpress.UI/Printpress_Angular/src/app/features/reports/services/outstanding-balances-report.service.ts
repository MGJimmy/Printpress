import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { OutstandingBalancesReportDto } from '../models/outstanding-balances-report.dto';

@Injectable({ providedIn: 'root' })
export class OutstandingBalancesReportService {
  constructor(private httpService: HttpService) {}

  getReport(): Observable<ApiResponseDto<OutstandingBalancesReportDto>> {
    return this.httpService.get<ApiResponseDto<OutstandingBalancesReportDto>>(
      ApiUrlResource.ReportsAPI.outstandingBalances,
    );
  }
}
