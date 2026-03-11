import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { SparePartSellingInvoiceCreateDto } from '../models/spare-part-selling-invoice-create.dto';

@Injectable({ providedIn: 'root' })
export class SparePartSellingInvoiceService {
  constructor(private httpService: HttpService) {}

  createInvoice(dto: SparePartSellingInvoiceCreateDto): Observable<any> {
    return this.httpService.post<any>(ApiUrlResource.SparePartSellingInvoiceAPI.add, dto);
  }
}
