import { Pipe, PipeTransform } from '@angular/core';
import { ServiceCategoryEnum } from '../models/service-category.enum';
import { ServiceCategoryArabicMap } from '../models/Mappers/service-category-arabic.map';

@Pipe({ 
    name: 'serviceCategoryArabic',
    standalone: true})
export class ServiceCategoryArabicPipe implements PipeTransform {
  transform(value: ServiceCategoryEnum): string {
    return ServiceCategoryArabicMap[value] || value;
  }
}
