import { Injectable } from '@angular/core';
import { HttpService } from '../../../core/services/http.service';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';

@Injectable({
  providedIn: 'root',
})
export class ReportViewerService {

  private apiUrl = 'https://localhost:7259/home/contact'; // Replace with your API URL

  constructor(private http:HttpService) {}

  getReport():Observable<BlobPart> { 
    let param ={}
    return this.http.getBlob(ApiUrlResource.Report.OrderReport, param);
  }
}
