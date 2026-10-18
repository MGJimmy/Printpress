import { Pipe, PipeTransform } from '@angular/core';

const LABELS: Record<string, string> = {
  Paper: 'ورق',
  Ink: 'حبر',
  InkSupplements: 'مستلزمات الحبر',
  CleaningTools: 'أدوات تنظيف',
  SparePart: 'قطع غيار',
  Other: 'أخرى',
};

export function inventoryItemCategoryArabic(value: string | null | undefined): string {
  if (!value) return '';
  return LABELS[value] ?? value;
}

@Pipe({
  name: 'inventoryItemCategoryArabic',
  standalone: true,
})
export class InventoryItemCategoryArabicPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    return inventoryItemCategoryArabic(value);
  }
}
