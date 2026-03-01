import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { PagingListDto } from '../../../core/models/paging-list.dto';
import { InventoryItemDto } from '../models/inventory-item.dto';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  constructor(private httpService: HttpService) {}

  getAll(pageSize: number, pageNumber: number): Observable<PagingListDto<InventoryItemDto>> {
    return this.httpService.get<PagingListDto<InventoryItemDto>>(
      ApiUrlResource.InventoryAPI.getAll,
      { pageSize, pageNumber }
    );
  }

  delete(id: number): Observable<any> {
    return this.httpService.delete<any>(ApiUrlResource.InventoryAPI.delete(id));
  }
}
