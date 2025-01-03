import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigurationService } from '../../../core/services/configration.service';
import { CustomerMockService } from './customer-mock.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Customer_interface } from '../models/Customer-interface';
import { ErrorHandlingService } from '../../../core/helpers/error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private appConfig: ConfigurationService,
    private customersMockService: CustomerMockService,
    private errorHandler: ErrorHandlingService
  ) {
    //assumed (api/customers)
    this.baseUrl = `${this.appConfig.getBaseUrl()}api/customers`;
  }

  getCustomers(): Observable<Customer_interface[]> {
    if (this.appConfig.useMock()) {
      return this.customersMockService.getCustomers();
    }
    return this.http.get<Customer_interface[]>(this.baseUrl).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  getCustomerById(id: number): Observable<Customer_interface | undefined> {
    if (this.appConfig.useMock()) {
      return this.customersMockService.getCustomerById(id);
    }
    return this.http.get<Customer_interface>(`${this.baseUrl}/${id}`).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  addCustomer(customer: Customer_interface): Observable<Customer_interface> {
    if (this.appConfig.useMock()) {
      return this.customersMockService.addCustomer(customer);
    }
    return this.http.post<Customer_interface>(this.baseUrl, customer).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  updateCustomer(customer: Customer_interface, id: number): Observable<Customer_interface> {
    const url = `${this.baseUrl}/${id}`;
    if (this.appConfig.useMock()) {
      return this.customersMockService.updateCustomer(customer, id);
    }
    return this.http.put<Customer_interface>(url, customer).pipe(
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