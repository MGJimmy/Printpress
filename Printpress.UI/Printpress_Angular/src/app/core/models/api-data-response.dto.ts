import { ApiResponseDto } from "./api-response.dto";

export interface ApiDataResponseDto<T> extends ApiResponseDto {
    data:T 
  }