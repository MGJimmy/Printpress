import { ServiceCategoryEnum } from "./service-category.enum";

export interface ServiceGetDto {
    id: string;
    name: string;
    price: number;
    serviceCategory: ServiceCategoryEnum;
}