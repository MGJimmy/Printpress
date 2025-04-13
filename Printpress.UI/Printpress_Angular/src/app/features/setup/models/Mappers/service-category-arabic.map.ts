import { ServiceCategoryEnum } from "../service-category.enum";

export const ServiceCategoryArabicMap: Record<ServiceCategoryEnum, string> = {
    [ServiceCategoryEnum.Printing]: 'طباعة',
    [ServiceCategoryEnum.Selling]: 'بيع',
    [ServiceCategoryEnum.Cutting]: 'قص',
    [ServiceCategoryEnum.Clueing]: 'لصق',
    [ServiceCategoryEnum.Stapling]: 'تدبيس'
  };
  