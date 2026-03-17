import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import {
  OrderGroupItemsResponseDto,
  ItemExecutionSummaryDto,
  ItemExecutionHistoryDto,
  ExecuteServiceRequestDto
} from '../models/execution/execution.dto';

@Injectable({ providedIn: 'root' })
export class ItemServiceExecutionService {
  constructor(private httpService: HttpService) {}

  getGroupItems(groupId: string): Observable<ApiResponseDto<OrderGroupItemsResponseDto>> {
    return this.httpService.get<ApiResponseDto<OrderGroupItemsResponseDto>>(
      ApiUrlResource.ItemServiceExecutionAPI.groupItems(groupId)
    );
  }

  getItemSummary(itemId: string): Observable<ApiResponseDto<ItemExecutionSummaryDto>> {
    return this.httpService.get<ApiResponseDto<ItemExecutionSummaryDto>>(
      ApiUrlResource.ItemServiceExecutionAPI.itemSummary(itemId)
    );
  }

  getItemHistory(itemId: string): Observable<ApiResponseDto<ItemExecutionHistoryDto>> {
    return this.httpService.get<ApiResponseDto<ItemExecutionHistoryDto>>(
      ApiUrlResource.ItemServiceExecutionAPI.itemHistory(itemId)
    );
  }

  execute(payload: ExecuteServiceRequestDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.ItemServiceExecutionAPI.execute, payload);
  }

  completeItem(itemId: string): Observable<any> {
    return this.httpService.post<any>(`${ApiUrlResource.ItemServiceExecutionAPI.completeItem}/${itemId}`, {});
  }
}
