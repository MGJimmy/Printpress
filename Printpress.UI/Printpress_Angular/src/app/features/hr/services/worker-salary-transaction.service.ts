import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { AddSalaryTransactionDto, WorkerSalaryTransactionDto } from '../models/worker.dto';

@Injectable({ providedIn: 'root' })
export class WorkerSalaryTransactionService {
  constructor(private httpService: HttpService) {}

  add(payload: AddSalaryTransactionDto): Observable<ApiResponseDto<WorkerSalaryTransactionDto>> {
    return this.httpService.post<ApiResponseDto<WorkerSalaryTransactionDto>>(
      ApiUrlResource.WorkerSalaryTransactionAPI.add,
      payload
    );
  }

  delete(id: string): Observable<any> {
    return this.httpService.delete<any>(ApiUrlResource.WorkerSalaryTransactionAPI.delete(id));
  }
}
