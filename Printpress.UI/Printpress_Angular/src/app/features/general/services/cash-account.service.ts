import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { CashAccountDto } from '../models/cash-account.dto';
import { ApiResponseDto } from '../../../core/models/api-response.dto';

@Injectable({ providedIn: 'root' })
export class CashAccountService {
  constructor(private httpService: HttpService) {}

  getAll(): Observable<ApiResponseDto<CashAccountDto[]>> {
    return this.httpService.get<ApiResponseDto<CashAccountDto[]>>(ApiUrlResource.CashAccountAPI.getAll);
  }

  getById(id: string): Observable<ApiResponseDto<CashAccountDto>> {
    return this.httpService.get<ApiResponseDto<CashAccountDto>>(ApiUrlResource.CashAccountAPI.getById(id));
  }
}
