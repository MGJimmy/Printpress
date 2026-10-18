export interface WorkerAttendanceRowDto {
  id: string;
  checkInAt: string;
  checkOutAt: string | null;
  hours: number | null;
  isVoided: boolean;
  voidReason: string | null;
  voidedAt: string | null;
}

export interface WorkerAttendanceListDto {
  workerId: string;
  isCheckedIn: boolean;
  rows: WorkerAttendanceRowDto[];
  totalHours: number;
}
