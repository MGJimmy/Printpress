import { Pipe, PipeTransform } from '@angular/core';
import { ServiceCategoryEnum } from '../models/service-category.enum';
import { ServiceCategoryArabicMap } from '../models/Mappers/service-category-arabic.map';

@Pipe({ 
    name: 'serviceCategoryArabic',
    standalone: true})
export class ServiceCategoryArabicPipe implements PipeTransform {
  transform(value: string | ServiceCategoryEnum): string {
    if (typeof value === 'string') {
      // If it's a string, convert it to enum
      const enumValue = ServiceCategoryEnum[value as keyof typeof ServiceCategoryEnum];
      return ServiceCategoryArabicMap[enumValue] || value;
    }
    return ServiceCategoryArabicMap[value] || ServiceCategoryEnum[value] || '';
  }
}
