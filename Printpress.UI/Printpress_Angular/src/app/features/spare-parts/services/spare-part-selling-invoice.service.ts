import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { SparePartSellingInvoiceCreateDto } from '../models/spare-part-selling-invoice-create.dto';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { SparePartSellingInvoiceListDto } from '../models/spare-part-invoice-list.dto';

@Injectable({ providedIn: 'root' })
export class SparePartSellingInvoiceService {
  constructor(private httpService: HttpService) {}

  createInvoice(dto: SparePartSellingInvoiceCreateDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.SparePartSellingInvoiceAPI.add, dto);
  }

  getAll(
    itemId?: string | null,
    dateFrom?: string,
    dateTo?: string,
  ): Observable<ApiResponseDto<SparePartSellingInvoiceListDto>> {
    const params: Record<string, string> = {};
    if (itemId) params['itemId'] = itemId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.httpService.get<ApiResponseDto<SparePartSellingInvoiceListDto>>(
      ApiUrlResource.SparePartSellingInvoiceAPI.getAll,
      params,
    );
  }
}
