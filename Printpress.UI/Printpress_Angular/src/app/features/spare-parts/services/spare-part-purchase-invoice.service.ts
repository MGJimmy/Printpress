import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { SparePartPurchaseInvoiceCreateDto } from '../models/spare-part-purchase-invoice-create.dto';
import { ApiResponseDto } from '../../../core/models/api-response.dto';

@Injectable({ providedIn: 'root' })
export class SparePartPurchaseInvoiceService {
  constructor(private httpService: HttpService) {}

  uploadFile(file: File): Observable<ApiResponseDto<{ filePath: string }>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.httpService.post<ApiResponseDto<{ filePath: string }>>(ApiUrlResource.FileUploadAPI.upload, formData);
  }

  createInvoice(dto: SparePartPurchaseInvoiceCreateDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.SparePartPurchaseInvoiceAPI.add, dto);
  }
}
