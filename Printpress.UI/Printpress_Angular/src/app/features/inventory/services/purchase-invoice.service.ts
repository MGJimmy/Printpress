import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { PurchaseInvoiceCreateDto } from '../models/purchase-invoice-create.dto';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { InventoryPurchaseInvoiceListDto, InventoryPurchaseInvoiceListItemDto } from '../models/inventory-document-list.dto';

@Injectable({ providedIn: 'root' })
export class PurchaseInvoiceService {
  constructor(private httpService: HttpService) {}

  uploadFile(file: File): Observable<ApiResponseDto<{ filePath: string }>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.httpService.post<ApiResponseDto<{ filePath: string }>>(ApiUrlResource.FileUploadAPI.upload, formData);
  }

  createInvoice(dto: PurchaseInvoiceCreateDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.PurchaseInvoiceAPI.add, dto);
  }

  getAll(
    pageNumber: number,
    pageSize: number,
    categoryId?: number | null,
    itemId?: string | null,
    dateFrom?: string,
    dateTo?: string,
    isVoided?: boolean | null,
    hasRemaining?: boolean | null,
    isGoodsReceived?: boolean | null,
  ): Observable<ApiResponseDto<InventoryPurchaseInvoiceListDto>> {
    const params: Record<string, string | number | boolean> = { pageNumber, pageSize };
    if (categoryId != null) params['categoryId'] = categoryId;
    if (itemId) params['itemId'] = itemId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    if (isVoided != null) params['isVoided'] = isVoided;
    if (hasRemaining != null) params['hasRemaining'] = hasRemaining;
    if (isGoodsReceived != null) params['isGoodsReceived'] = isGoodsReceived;
    return this.httpService.get<ApiResponseDto<InventoryPurchaseInvoiceListDto>>(
      ApiUrlResource.PurchaseInvoiceAPI.getAll,
      params,
    );
  }

  getById(id: string): Observable<ApiResponseDto<InventoryPurchaseInvoiceListItemDto>> {
    return this.httpService.get<ApiResponseDto<InventoryPurchaseInvoiceListItemDto>>(
      ApiUrlResource.PurchaseInvoiceAPI.getById(id),
    );
  }

  pay(id: string, amount: number, note?: string): Observable<ApiResponseDto<unknown>> {
    return this.httpService.post<ApiResponseDto<unknown>>(ApiUrlResource.PurchaseInvoiceAPI.pay(id), {
      amount,
      note: note ?? '',
    });
  }

  receive(id: string): Observable<ApiResponseDto<unknown>> {
    return this.httpService.post<ApiResponseDto<unknown>>(ApiUrlResource.PurchaseInvoiceAPI.receive(id), {});
  }

  void(id: string, reason?: string): Observable<ApiResponseDto<unknown>> {
    return this.httpService.post<ApiResponseDto<unknown>>(ApiUrlResource.PurchaseInvoiceAPI.void(id), { reason: reason ?? '' });
  }
}
