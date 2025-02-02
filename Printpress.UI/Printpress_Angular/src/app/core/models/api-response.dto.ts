import { PagingListDto } from './paging-list.dto';

export interface ApiResponseDto<T> extends BaseRespons {
  data: T;
}

export interface ApiPagingResponseDto<T> extends BaseRespons {
  data: PagingListDto<T>;
}

interface BaseRespons {
  status: ResponseStatus;
  message: string;
  isSuccess: boolean;
  errors?: string[];
}

export enum ResponseStatus {
  Success = 200,
  ExceptionError = 500,
  ValidationFailure = 400,
}
