export interface ServiceProgressDto {
  serviceCategoryId: string;
  serviceCategoryName: string;
  executed: number;
  total: number;
  isCompleted: boolean;
}

export interface ItemWithServiceProgressDto {
  id: string;
  name: string;
  quantity: number;
  status: string;
  serviceProgresses: ServiceProgressDto[];
}

export interface OrderGroupItemsResponseDto {
  groupId: string;
  orderId: string;
  groupName: string;
  groupStatus: string;
  executionType: string;
  groupServices: ServiceProgressDto[];
  items: ItemWithServiceProgressDto[];
}

export interface ItemExecutionSummaryDto {
  itemId: string;
  itemName: string;
  quantity: number;
  status: string;
  groupId: string;
  serviceProgresses: ServiceProgressDto[];
}

export interface ItemExecutionRecordDto {
  id: string;
  workerName: string;
  serviceCategoryName: string;
  quantity: number;
  executionDate: string;
  notes: string;
}

export interface ItemExecutionHistoryDto {
  itemId: string;
  itemName: string;
  quantity: number;
  status: string;
  groupId: string;
  groupName: string;
  serviceProgresses: ServiceProgressDto[];
  executionRecords: ItemExecutionRecordDto[];
}

export interface WorkerExecutionRowDto {
  workerId: string;
  quantity: number;
}

export interface ExecuteServiceRequestDto {
  orderItemId: string;
  serviceCategoryId: string;
  executionDate: string;
  notes: string;
  workers: WorkerExecutionRowDto[];
}

export const ItemStatusLabels: Record<string, string> = {
  New: 'جديد',
  InProgress: 'قيد التنفيذ',
  Completed: 'مكتمل'
};

export const GroupStatusLabels: Record<string, string> = {
  New: 'جديد',
  InProgress: 'قيد التنفيذ',
  Completed: 'مكتمل',
  Delivered: 'تم التسليم'
};
