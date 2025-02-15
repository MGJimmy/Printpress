import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ClientGetDto } from '../models/client-get.dto';
import { ClientUpsertDto } from '../models/client-upsert.dto';
import { ConfigurationService } from '../../../core/services/configuration.service';
import { ClientMockService } from './client-mock.service';
import { ApiResponseDto } from '../../../core/models/api-response.dto';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  constructor(
    private httpService:HttpService,
    private configurationService : ConfigurationService,
    private clientMockService: ClientMockService
  ) {
  }

  getByPage(pageNumber:number, pageSize:number): Observable<ClientGetDto[]> {
    if(this.configurationService.useMock()){
      return this.clientMockService.getCustomers();
    }else
    return this.httpService.get<ClientGetDto[]>(ApiUrlResource.ClientAPI.getByPage, {pageNumber:pageNumber, pageSize:pageSize});
  }

  getById(id: number): Observable<ClientGetDto> {
    if(this.configurationService.useMock()){
      return this.clientMockService.getCustomerById(id);
    }else
    return this.httpService.get<ClientGetDto>(ApiUrlResource.ClientAPI.getById, {id:id})
  }

  getAll(): Observable<ApiResponseDto<ClientGetDto[]>>{
     return this.httpService.get<ApiResponseDto<ClientGetDto[]>>(ApiUrlResource.ClientAPI.getAll);
  }

  add(client: ClientUpsertDto): Observable<ClientUpsertDto> {
    if(this.configurationService.useMock()){
      return this.clientMockService.addCustomer(client);
    }else
    return this.httpService.post<ClientUpsertDto>(ApiUrlResource.ClientAPI.add, client);
  }

  update(client: ClientUpsertDto, id: number): Observable<ClientUpsertDto> {
    if(this.configurationService.useMock()){
      return this.clientMockService.updateCustomer(client, id);
    }else
    return this.httpService.put<ClientUpsertDto>(ApiUrlResource.ClientAPI.update(id), client);
  }

  delete(id: number): Observable<string> {
    if(this.configurationService.useMock()){
      return this.clientMockService.deleteCustomer(id);
    }else
    return this.httpService.delete(ApiUrlResource.ClientAPI.delete(id))
   }
}
