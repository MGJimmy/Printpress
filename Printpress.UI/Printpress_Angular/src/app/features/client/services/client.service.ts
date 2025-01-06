import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigurationService } from '../../../core/services/configration.service';
import { ClientMockService } from './client-mock.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ClientAddUpdateDto } from '../models/ClientAddUpdate.Dto';
import { ErrorHandlingService } from '../../../core/helpers/error-handling.service';
import { HttpService } from '../../../core/services/http.service';
import { ClientGetDto } from '../models/ClientGet.Dto';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';

@Injectable({
  providedIn: 'root'
})
export class CleintService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private appConfig: ConfigurationService,
    private customersMockService: ClientMockService,
    private errorHandler: ErrorHandlingService,
    private httpService:HttpService
  ) {
    //assumed (api/customers)
    this.baseUrl = `${this.appConfig.getBaseUrl()}api/customers`;
  }

  getCustomers(): Observable<ClientAddUpdateDto[]> {
    if (this.appConfig.useMock()) {
      return this.customersMockService.getCustomers();
    }
    return this.http.get<ClientAddUpdateDto[]>(this.baseUrl).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  getCustomerById(id: number): Observable<ClientAddUpdateDto | undefined> {
    if (this.appConfig.useMock()) {
      return this.customersMockService.getCustomerById(id);
    }

    return this.httpService.get<ClientGetDto>(ApiUrlResource.ClientAPI.getClientById, {id:id})
  }

  addCustomer(customer: ClientAddUpdateDto): Observable<ClientAddUpdateDto> {
    if (this.appConfig.useMock()) {
      return this.customersMockService.addCustomer(customer);
    }
    return this.http.post<ClientAddUpdateDto>(this.baseUrl, customer).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  updateCustomer(customer: ClientAddUpdateDto, id: number): Observable<ClientAddUpdateDto> {
    const url = `${this.baseUrl}/${id}`;
    if (this.appConfig.useMock()) {
      return this.customersMockService.updateCustomer(customer, id);
    }
    return this.http.put<ClientAddUpdateDto>(url, customer).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  // Delete customer
  deleteCustomer(id: number): Observable<string> {
    if (this.appConfig.useMock()) {
      return this.customersMockService.deleteCustomer(id);
    }
    return this.http.delete(`${this.baseUrl}/${id}`, { responseType: 'text' }).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }
}
