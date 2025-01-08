import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ConfigurationService } from '../../../core/services/configuration.service';
import { ClientMockService } from './client-mock.service';
import { HttpService } from '../../../core/services/http.service';
import { catchError } from 'rxjs/operators';
import { Client_interface } from '../models/Client-interface';
import { ErrorHandlingService } from '../../../core/helpers/error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  readonly endPoint: string = 'api/clients';

  constructor(
    private httpService: HttpService,
    private appConfig: ConfigurationService,
    private ClientMockService: ClientMockService,
    private errorHandler: ErrorHandlingService
  ) {
  }

  getClients(): Observable<Client_interface[]> {
    if (this.appConfig.useMock()) {
      return this.ClientMockService.getClients();
    }
    return this.httpService.get<Client_interface[]>(this.endPoint).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  getClientById(id: number): Observable<Client_interface | undefined> {
    if (this.appConfig.useMock()) {
      return this.ClientMockService.getClientById(id);
    }
    return this.httpService.get<Client_interface>(`${this.endPoint}/${id}`).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  addClient(client: Client_interface): Observable<Client_interface> {
    if (this.appConfig.useMock()) {
      return this.ClientMockService.addClient(client);
    }
    return this.httpService.post<Client_interface>(this.endPoint, client).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  updateClient(client: Client_interface, id: number): Observable<Client_interface> {
    if (this.appConfig.useMock()) {
      return this.ClientMockService.updateClient(client, id);
    }
    return this.httpService.put<Client_interface>(this.endPoint, client).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }

  // Delete customer
  deleteClient(id: number): Observable <any> {
    if (this.appConfig.useMock()) {
      return this.ClientMockService.deleteClient(id);
    }
    return this.httpService.delete(`${this.endPoint}/${id}`).pipe(
      catchError((error) => this.errorHandler.handleError(error))
    );
  }
}
