import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { SparePartSellingInvoiceCreateDto } from '../models/spare-part-selling-invoice-create.dto';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { SparePartSellingInvoiceListDto, SparePartSellingInvoiceListItemDto } from '../models/spare-part-invoice-list.dto';

@Injectable({ providedIn: 'root' })
export class SparePartSellingInvoiceService {
  constructor(private httpService: HttpService) {}

  createInvoice(dto: SparePartSellingInvoiceCreateDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.SparePartSellingInvoiceAPI.add, dto);
  }

  getAll(
    pageNumber: number,
    pageSize: number,
    itemId?: string | null,
    dateFrom?: string,
    dateTo?: string,
    isVoided?: boolean | null,
  ): Observable<ApiResponseDto<SparePartSellingInvoiceListDto>> {
    const params: Record<string, string | number | boolean> = { pageNumber, pageSize };
    if (itemId) params['itemId'] = itemId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    if (isVoided != null) params['isVoided'] = isVoided;
    return this.httpService.get<ApiResponseDto<SparePartSellingInvoiceListDto>>(
      ApiUrlResource.SparePartSellingInvoiceAPI.getAll,
      params,
    );
  }

  getById(id: string): Observable<ApiResponseDto<SparePartSellingInvoiceListItemDto>> {
    return this.httpService.get<ApiResponseDto<SparePartSellingInvoiceListItemDto>>(
      ApiUrlResource.SparePartSellingInvoiceAPI.getById(id),
    );
  }

  void(id: string, reason?: string): Observable<ApiResponseDto<unknown>> {
    return this.httpService.post<ApiResponseDto<unknown>>(ApiUrlResource.SparePartSellingInvoiceAPI.void(id), { reason: reason ?? '' });
  }
}
