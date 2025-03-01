import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ClientGetDto } from '../models/client-get.dto';
import { ClientUpsertDto } from '../models/client-upsert.dto';
import { ConfigurationService } from '../../../core/services/configuration.service';
import { ClientMockService } from './client-mock.service';
import { ApiPagingResponseDto, ApiResponseDto } from '../../../core/models/api-response.dto';

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

  getByPage(pageNumber:number, pageSize:number): Observable<ApiPagingResponseDto<ClientGetDto>> { 
    return this.httpService.get<ApiPagingResponseDto<ClientGetDto>>(ApiUrlResource.ClientAPI.getByPage, {pageNumber:pageNumber, pageSize:pageSize});
  }

  getById(id: number): Observable<ApiResponseDto< ClientGetDto>> {
    return this.httpService.get<ApiResponseDto<ClientGetDto>>(ApiUrlResource.ClientAPI.getById, {id:id})
  }

  getAll(): Observable<ApiResponseDto<ClientGetDto[]>>{
     return this.httpService.get<ApiResponseDto<ClientGetDto[]>>(ApiUrlResource.ClientAPI.getAll);
  }

  add(client: ClientUpsertDto): Observable<ApiResponseDto<ClientUpsertDto>> {
    return this.httpService.post<ApiResponseDto<ClientUpsertDto>>(ApiUrlResource.ClientAPI.add, client);
  }

  update(client: ClientUpsertDto, id: number): Observable<ApiResponseDto<ClientUpsertDto>> {
    return this.httpService.put<ApiResponseDto<ClientUpsertDto>>(ApiUrlResource.ClientAPI.update(id), client);
  }

  delete(id: number): Observable<string> {
    return this.httpService.delete(ApiUrlResource.ClientAPI.delete(id))
   }
}
