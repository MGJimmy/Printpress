import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/services/http.service';
import { ApiUrlResource } from '../../../core/resources/api-urls.resource';
import { ApiResponseDto } from '../../../core/models/api-response.dto';
import { TodoItemAddDto, TodoListDto } from '../models/todo.dto';

@Injectable({ providedIn: 'root' })
export class TodoService {
  constructor(private http: HttpService) {}

  getList(): Observable<ApiResponseDto<TodoListDto>> {
    return this.http.get<ApiResponseDto<TodoListDto>>(ApiUrlResource.TodoItemAPI.getList);
  }

  add(payload: TodoItemAddDto): Observable<ApiResponseDto<unknown>> {
    return this.http.post<ApiResponseDto<unknown>>(ApiUrlResource.TodoItemAPI.add, payload);
  }

  complete(id: string): Observable<ApiResponseDto<unknown>> {
    return this.http.post<ApiResponseDto<unknown>>(ApiUrlResource.TodoItemAPI.complete(id), {});
  }
}
