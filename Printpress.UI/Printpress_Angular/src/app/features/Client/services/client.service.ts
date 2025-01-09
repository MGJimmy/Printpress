import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorHandlingService } from '../../../core/helpers/error-handling.service';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ClientGetDto } from '../models/client-get.Dto';
import { ClientUpsertDto } from '../models/client-upsert.Dto';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  constructor(
    private httpService:HttpService
  ) {
  }

  getByPage(pageNumber:number, pageSize:number): Observable<ClientGetDto[]> {
    return this.httpService.get<ClientGetDto[]>(ApiUrlResource.ClientAPI.getByPage, {pageNumber:pageNumber, pageSize:pageSize});
  }

  getById(id: number): Observable<ClientGetDto> {
    return this.httpService.get<ClientGetDto>(ApiUrlResource.ClientAPI.getById, {id:id})
  }

  add(client: ClientUpsertDto): Observable<ClientUpsertDto> {
    return this.httpService.post<ClientUpsertDto>(ApiUrlResource.ClientAPI.add, client);
  }

  update(client: ClientUpsertDto, id: number): Observable<ClientUpsertDto> {
    return this.httpService.put<ClientUpsertDto>(ApiUrlResource.ClientAPI.update(id), client);
  }

  delete(id: number): Observable<string> {
    return this.httpService.delete(ApiUrlResource.ClientAPI.delete(id))
   }
}
