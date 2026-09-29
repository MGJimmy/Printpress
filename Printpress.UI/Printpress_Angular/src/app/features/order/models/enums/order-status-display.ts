import { OrderStatus } from './order-status.enum';

const ORDER_STATUS_BY_NUMBER: Record<string, string> = {
  '1': OrderStatus.New,
  '2': OrderStatus.InProgress,
  '4': OrderStatus.Closed,
  '5': OrderStatus.Draft,
};

export function normalizeOrderStatus(status: string | number | null | undefined): string {
  if (status === null || status === undefined || status === '') {
    return '';
  }

  return ORDER_STATUS_BY_NUMBER[String(status)] ?? String(status);
}

export function isOrderStatus(status: string | number | null | undefined, ...expected: string[]): boolean {
  return expected.includes(normalizeOrderStatus(status));
}

const ORDER_STATUS_I18N_KEY: Record<string, string> = {
  Draft: 'orders.status_draft',
  New: 'orders.status_new',
  InProgress: 'orders.status_in_progress',
  Closed: 'orders.status_closed',
};

export function orderStatusI18nKey(status: string | number | null | undefined): string {
  return ORDER_STATUS_I18N_KEY[normalizeOrderStatus(status)] ?? 'orders.status_unknown';
}

export function orderStatusBadgeClass(status: string | number | null | undefined): string {
  switch (normalizeOrderStatus(status)) {
    case OrderStatus.Closed:
      return 'bg-dark';
    case OrderStatus.InProgress:
      return 'bg-warning text-dark';
    case OrderStatus.New:
      return 'bg-info';
    case OrderStatus.Draft:
      return 'bg-secondary';
    default:
      return 'bg-secondary';
  }
}
