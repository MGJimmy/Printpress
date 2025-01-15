export interface PagingListDto<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
  }