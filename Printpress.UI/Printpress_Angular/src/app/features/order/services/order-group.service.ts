import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ItemGetDto } from '../models/item/item-get.Dto';
import { HttpService } from '../../../core/services/http.service';

@Injectable({
  providedIn: 'root'
})
export class OrderGroupService {

  constructor(private httpService: HttpService) { }

  public getAllGroupItems(): Observable<ItemGetDto[]> {
    return this.httpService.get('');
  }
}
