const STATUS_BY_NUMBER: Record<string, string> = {
  '1': 'New',
  '2': 'InProgress',
  '3': 'Completed',
  '4': 'Delivered',
};

export function normalizeStatus(status: string | number | null | undefined): string {
  if (status === null || status === undefined || status === '') {
    return '';
  }

  return STATUS_BY_NUMBER[String(status)] ?? String(status);
}

export function isStatus(status: string | number | null | undefined, ...expected: string[]): boolean {
  return expected.includes(normalizeStatus(status));
}

export const STATUS_I18N_KEY: Record<string, string> = {
  New: 'orders.status_new',
  InProgress: 'orders.status_in_progress',
  Completed: 'orders.status_completed',
  Delivered: 'orders.status_delivered',
};

export function statusI18nKey(status: string | number | null | undefined): string {
  return STATUS_I18N_KEY[normalizeStatus(status)] ?? 'orders.status_unknown';
}

export function statusBadgeClass(status: string | number | null | undefined): string {
  switch (normalizeStatus(status)) {
    case 'Delivered':
      return 'bg-info';
    case 'Completed':
      return 'bg-success';
    case 'InProgress':
      return 'bg-warning text-dark';
    case 'New':
      return 'bg-secondary';
    default:
      return 'bg-secondary';
  }
}
