import { Injectable } from '@angular/core';
import { HttpService } from '../../../core/services/http.service';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';


@Injectable({
  providedIn: 'root',
})
export class ReportViewerService {
  constructor(private http: HttpService) {}

  getReport(queryParams: { [key: string]: any }): Observable<BlobPart> {
    return this.http.getBlob(ApiUrlResource.Report.OrderReport, queryParams);
  }
  
  
}
