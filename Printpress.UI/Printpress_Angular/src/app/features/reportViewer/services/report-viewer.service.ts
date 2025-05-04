import { Injectable } from '@angular/core';
import { HttpService } from '../../../core/services/http.service';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';

export enum ReportType {
  Invoice = 'invoice',
}

@Injectable({
  providedIn: 'root',
})
export class ReportViewerService {
  constructor(private http: HttpService) {}

  getReport(reportType: ReportType, id: number): Observable<BlobPart> {
    let param = {};
    switch (reportType) {
      case ReportType.Invoice:
        return this.http.getBlob(ApiUrlResource.Report.OrderReport(id), param);

      default:
        throw new Error('Invalid report type');
    }
  }
}
