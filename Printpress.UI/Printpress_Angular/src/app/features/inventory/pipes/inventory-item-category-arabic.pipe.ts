import { Pipe, PipeTransform } from '@angular/core';

const LABELS: Record<string, string> = {
  Paper: 'ورق',
  Ink: 'حبر',
  InkSupplements: 'مستلزمات الحبر',
  CleaningTools: 'أدوات تنظيف',
  SparePart: 'قطع غيار',
  Other: 'أخرى',
};

@Pipe({
  name: 'inventoryItemCategoryArabic',
  standalone: true,
})
export class InventoryItemCategoryArabicPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '';
    return LABELS[value] ?? value;
  }
}
