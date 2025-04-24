import { ServiceCategoryEnum } from "../service-category.enum";

export const ServiceCategoryArabicMap: Record<ServiceCategoryEnum, string> = {
    [ServiceCategoryEnum.Printing]: 'طباعة',
    [ServiceCategoryEnum.Stapling]: 'تدبيس',
    [ServiceCategoryEnum.Clueing]: 'لصق',
    [ServiceCategoryEnum.Cutting]: 'قص',
    [ServiceCategoryEnum.Selling]: 'بيع'
};
