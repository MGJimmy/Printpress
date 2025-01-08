import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { firstValueFrom, Observable } from "rxjs";
import { ConfigurationService } from "./configuration.service";
import { Configuration } from "../models/configration.model";

@Injectable({
    providedIn: 'root',

})
export class HttpService {

    private defaultHeaders : HttpHeaders;

    constructor(private http: HttpClient ,private configrationService:ConfigurationService) {
        this.defaultHeaders = new HttpHeaders();
        this.defaultHeaders = this.defaultHeaders.set('showLoader','true');
     }


    public get<T>(url: string, queryParams?: { [param: string]: string | number | boolean }, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        const params = new HttpParams({ fromObject: queryParams || {} });
        return this.http.get<T>(this.setUrl(url), { params, headers: requestHeaders });
    }

    public post<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        return this.http.post<T>(this.setUrl(url), body, { headers: requestHeaders });
    }

    public put<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        return this.http.put<T>(this.setUrl(url), body, { headers: requestHeaders });
    }

    public delete<T>(url: string, headers?: HttpHeaders): Observable<T> {
        const requestHeaders = headers || this.defaultHeaders;
        return this.http.delete<T>(this.setUrl(url), { headers: requestHeaders });
    }

    private setUrl(url:string){
     return this.configrationService.getConfiguration().apiUrl + url;
    }



}
