import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { firstValueFrom, Observable } from "rxjs";
import { ConfigurationService } from "./configration.service";
import { Configuration } from "../models/configration.model";

@Injectable({
    providedIn: 'root',
    
})
export class HttpService {

    constructor(private http: HttpClient ,private configrationService:ConfigurationService) { }

    private defaultHeaders = new HttpHeaders({
        'showLoader': 'true'
    });

    public get<T>(url: string, queryParams?: { [param: string]: string | number | boolean }, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        const params = new HttpParams({ fromObject: queryParams || {} });
        return this.http.get<T>(url, { params, headers: requestHeaders });
    }

    public post<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        return this.http.post<T>(url, body, { headers: requestHeaders });
    }

    public put<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        return this.http.put<T>(url, body, { headers: requestHeaders });
    }

    public delete<T>(url: string, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        return this.http.delete<T>(url, { headers: requestHeaders });
    }

    private async setUrl(url:string){

       let configuration:Configuration = await this.configrationService.getConfiguration();
       return configuration.apiUrl + url;
    }

}