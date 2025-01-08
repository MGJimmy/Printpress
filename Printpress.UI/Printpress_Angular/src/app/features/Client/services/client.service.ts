import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigurationService } from '../../../core/services/configration.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorHandlingService } from '../../../core/helpers/error-handling.service';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ClientUpsertDto } from '../models/client-upsert.dto';
import { ClientGetDto } from '../models/client-get.dto';

@Injectable({
  providedIn: 'root'
})
export class CleintService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private appConfig: ConfigurationService,
    private errorHandler: ErrorHandlingService,
    private httpService:HttpService
  ) {
    //assumed (api/customers)
    this.baseUrl = `${this.appConfig.getBaseUrl()}api/customers`;
  }

  getCustomers(): Observable<ClientUpsertDto[]> {
    // if (this.appConfig.useMock()) {
    //   return this.customersMockService.getCustomers();
    // }
    return this.http.get<ClientUpsertDto[]>(this.baseUrl).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  getCustomerById(id: number): Observable<ClientUpsertDto | undefined> {
    // if (this.appConfig.useMock()) {
    //   return this.customersMockService.getCustomerById(id);
    // }

    return this.httpService.get<ClientGetDto>(ApiUrlResource.ClientAPI.getById, {id:id})
  }

  addCustomer(customer: ClientUpsertDto): Observable<ClientUpsertDto> {
    // if (this.appConfig.useMock()) {
    //   return this.customersMockService.addCustomer(customer);
    // }
    return this.http.post<ClientUpsertDto>(this.baseUrl, customer).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  updateCustomer(customer: ClientUpsertDto, id: number): Observable<ClientUpsertDto> {
    const url = `${this.baseUrl}/${id}`;
    // if (this.appConfig.useMock()) {
    //   return this.customersMockService.updateCustomer(customer, id);
    // }
    return this.http.put<ClientUpsertDto>(url, customer).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  // Delete customer
  deleteCustomer(id: number): Observable<string> {
    // if (this.appConfig.useMock()) {
    //   return this.customersMockService.deleteCustomer(id);
    // }
    return this.http.delete(`${this.baseUrl}/${id}`, { responseType: 'text' }).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }
}
